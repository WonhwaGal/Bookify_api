using Dapper;
using Bookify.Infrastructure.Services;
using Bookify.Infrastructure.Services.Impl;
using Bogus.DataSets;
using Bookify.Models.Results;
using Bookify.Infrastructure;
using Bookify.Domain;
using Microsoft.Extensions.Options;
using Bookify.Options;
using SmsServiceNamespace;
using Bookify.Services;
using System.Collections.Generic;

namespace Bookify.Models.Services.Impl
{
    public class ReviewRepository(
        IDefaultSmsClient defaultSmsClient,
        IBookingRepository bookingRepository,
        ISqlConnectionFactory sqlConnectionFactory,
        ICacheService cacheService,
        ApplicationDbContext dbContext) : IReviewRepository
    {
        public async Task<Result<Guid>> CreateReview(Guid bookingId, int rating, string? comment)
        {
            var booking = bookingRepository.GetById(bookingId);
            if(booking == null)
            {
                return Result.Failure<Guid>(BookingError.NotFound);
            }

            if (booking.Status != BookingStatus.Completed)
            {
                return Result.Failure<Guid>(BookingError.NotCompleted);
            }

            var bookingAlreadyHasReview = dbContext.Reviews.Any(rev => rev.BookingId == bookingId);
            if (bookingAlreadyHasReview)
            {
                return Result.Failure<Guid>(BookingError.ExistingReview);
            }

            var review = Review.PublishReview(
                booking.ApartmentId,
                booking.Id,
                booking.UserId,
                rating,
                comment);

            dbContext.Reviews.Add(review);
            dbContext.SaveChanges();

            var smsResponse = await defaultSmsClient.SendSmsAsync(review.Id, "Bookify", "user.PhoneNumber");
            if (smsResponse.IsSuccess)
            {
                var notificationSmsId = smsResponse.Value;
            }
            return review.Id;
        } 

        public Result<IReadOnlyList<Review>> GetByPer(Guid appartID, DateOnly? startDate, DateOnly? endDate, byte? rating)
        {
            DateOnly startD = startDate ?? DateOnly.FromDayNumber(1000);
            DateOnly endD = endDate ?? DateOnly.FromDateTime(DateTime.Today);

            var cacheKey = $"review-by-per-{appartID}-{startD.ToShortDateString()}-{endD.ToShortDateString()}-{rating}";
            var cacheData = cacheService.GetAsync<IReadOnlyList<Review>>(cacheKey).Result;
            if(cacheData is not null)
            {
                return cacheData.ToList();
            }

            if (startDate >= endDate)
                return new List<Review>();

            using var connection = sqlConnectionFactory.CreateConnection();

            const string sql = """
                           SELECT
                               rev.Id AS Id,
                               rev.Rating AS Rating,
                               rev.Comment AS Comment,
                               rev.ApartmentId AS ApartmentId,
                               rev.CreatedOnUtc AS CreatedOnUtc
                           FROM reviews AS rev
                           WHERE 
                               rev.ApartmentId = @appartID AND
                               rev.Rating = CASE WHEN @rating IS NULL THEN rev.Rating ELSE @rating END AND
                               rev.CreatedOnUtc >= CASE WHEN @startD IS NULL THEN rev.CreatedOnUtc ELSE @startD END AND
                               rev.CreatedOnUtc <= CASE WHEN @endD IS NULL THEN rev.CreatedOnUtc ELSE @endD END
                           """;

            var reviews = connection
                .Query<Review>(
                sql,
                new
                {
                    appartID,
                    startD,
                    endD,
                    rating
                });

            cacheService.SetAsync(cacheKey, reviews, TimeSpan.FromSeconds(30));

            return reviews.ToList();
        }

        public int Create(Review item)
        {
            throw new NotImplementedException();
        }

        public ICollection<Review> GetAll()
        {
            throw new NotImplementedException();
        }

        public Review? GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public int Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public int Update(Review item)
        {
            throw new NotImplementedException();
        }
    }
}