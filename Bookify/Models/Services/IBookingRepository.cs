using Bookify.Domain;

namespace Bookify.Models.Services
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Result<Guid> Reserve(Guid apartmentId, string identityId, DateOnly dateFrom, DateOnly dateTo);

        bool IsOverlapping(Apartment apartment, DateOnly dateFrom, DateOnly dateTo);
        
        Result<IReadOnlyList<Booking>> GetByPer(
            string? identityId, DateOnly? startDate, DateOnly? endDate, BookingStatus? bookingStatus);
    }
}
