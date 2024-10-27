using Bookify.Controllers;
using Bookify.Domain;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Services;
using Bookify.Models.Results;
using Bookify.Services;
using Bookify.Services.Impl;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bookify.Models.Services.Impl
{
    public class BookingRepository(
        PricingService pricingService,
        IApartmentRepository apartmentRepository,
        IUserRepository userRepository,
        ISqlConnectionFactory sqlConnectionFactory,
        ICacheService cacheService,
        ApplicationDbContext dbContext,
        ILogger<BookingRepository> logger) : IBookingRepository
    {
        private static readonly BookingStatus[] ActiveBookingStatuses =
{
            BookingStatus.Reserved,
            BookingStatus.Confirmed
        };

        public bool IsOverlapping(Apartment apartment, DateOnly dateFrom, DateOnly dateTo)
        {
            return dbContext.Bookings.Any(
                booking => booking.ApartmentId == apartment.Id &&
                booking.DateFrom <= dateTo.ToDateTime(TimeOnly.MinValue) &&
                booking.DateTo >= dateFrom.ToDateTime(TimeOnly.MinValue) &&
                ActiveBookingStatuses.Contains(booking.Status));
        }

        public Result<Guid> Reserve(Guid apartmentId, string identityId, DateOnly dateFrom, DateOnly dateTo)
        {
            var user = userRepository.GetByIdentityId(identityId);

            logger.LogInformation("Проверка параметров бронирования");

            if(user == null)
            {
                return Result.Failure<Guid>(UserError.NotFound);
            }

            var apartment = apartmentRepository.GetById(apartmentId);
            if(apartment == null)
            {
                return Result.Failure<Guid>(ApartmentError.NotFound);
            }

            logger.LogInformation("Проверка дат");
            if (dateFrom > dateTo)
            {
                return Result.Failure<Guid>(BookingError.Date);
            }

            if(IsOverlapping(apartment, dateFrom, dateTo))
            {
                return Result.Failure<Guid>(BookingError.Overlap);
            }

            var booking = Booking.Reserve(
                pricingService,
                apartment,
                user.Value.Id,
                dateFrom,
                dateTo);

            try
            {
                logger.LogInformation("Добавление бронирования в базу данных");
                dbContext.Bookings.Add(booking);
                dbContext.SaveChanges();
            }
            catch(DbUpdateConcurrencyException exc)
            {
                return Result.Failure<Guid>(BookingError.ConcurrencyOverlap);
            }

            return booking.Id;
        }

        public Booking? GetById(Guid id)
        {
            return dbContext.Bookings.FirstOrDefault(booking => booking.Id == id);
        }

        public Result<IReadOnlyList<Booking>> GetByPer(
            string? identityId, DateOnly? startDate, DateOnly? endDate, BookingStatus? bookingStatus)
        {
            DateOnly startD = startDate ?? DateOnly.FromDayNumber(1000);
            DateOnly endD = endDate ?? DateOnly.FromDateTime(DateTime.Today);

            var status = bookingStatus is null ? "none" : bookingStatus.ToString();
            var cacheKey = $"booking-by-per-{startD.ToShortDateString()}-{endD.ToShortDateString()}-{status}";
            var cacheData = cacheService.GetAsync<IReadOnlyList<Booking>>(cacheKey).Result;
            if (cacheData is not null)
            {
                return cacheData.ToList();
            }

            var user = userRepository.GetByIdentityId(identityId);
            var userId = user.Value.Id;

            if (startDate >= endDate)
                return new List<Booking>();

            var connection = sqlConnectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    b.Id AS Id,
                    b.UserId AS UserId,
                    b.ApartmentId AS ApartmentId,
                    b.Status AS Status,
                    b.TotalPrice AS TotalPrice,
                    b.DateFrom AS DateFrom,
                    b.DateTo AS DateTo
                FROM bookings AS b
                WHERE
                    b.UserId = @userId AND
                    b.DateFrom >= CASE WHEN @startD IS NULL THEN b.DateFrom ELSE @startD END AND
                    b.DateTo <= CASE WHEN @endD IS NULL THEN b.DateTo ELSE @endD END AND
                    b.Status = CASE WHEN @bookingStatus IS NULL THEN b.Status ELSE @bookingStatus END
                """;

            var bookings = connection
                .Query<Booking>(
                sql,
                new
                {
                    userId,
                    startD,
                    endD,
                    bookingStatus
                });

            cacheService.SetAsync(cacheKey, bookings, TimeSpan.FromMinutes(2));

            return bookings.ToList();
        }

        public int Update(Booking item)
        {
            throw new NotImplementedException();
        }

        public int Create(Booking item)
        {
            throw new NotImplementedException();
        }

        public ICollection<Booking> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}