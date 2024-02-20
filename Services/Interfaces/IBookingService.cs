using Model.Models;
using Model.ViewModel;

namespace Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseManager> GetById(int id);
        Task<ResponseManager> GetAllBookings();
        Task<ResponseManager> GetBookings(int page, int pageSize, string nameSearch, string sortBy, string sortOrder);
        Task<ResponseManager> DeleteBooking(int id);
        Task<ResponseManager> AddOrUpdateBooking(BookingVM bookingModel);
    }
}