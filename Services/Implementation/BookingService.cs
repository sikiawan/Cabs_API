using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using Model.ViewModel;
using Services.Interfaces;
namespace Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ResponseManager> GetById(int id)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (booking == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Booking not found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = booking
                };
            }
            catch (Exception ex)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = ex.Message
                };
            }
        }
        public async Task<ResponseManager> GetAllBookings()
        {
            try
            {
                var dbBookings = await _unitOfWork.Bookings.GetAll();
                var bookings = _mapper.Map<List<BookingVM>>(dbBookings);

                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = bookings
                };
            }
            catch (Exception ex)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = ex.Message
                };
            }
        }

        public async Task<ResponseManager> GetBookings(int page, int pageSize, string nameSearch, string sortBy, string sortOrder)
        {
            try
            {
                var bookingDb = await _unitOfWork.Bookings.GetAll();
                
                var query = bookingDb.OrderByDescending(x=> x.Id).AsQueryable();
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "name":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                            break;
                        case "email":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email);
                            break;
                        case "whatsapp":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.WhatsAppWithCC) : query.OrderByDescending(c => c.WhatsAppWithCC);
                            break;
                        case "vehicletype":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.VehicleType) : query.OrderByDescending(c => c.VehicleType);
                            break;
                        case "pickuplocation":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.PickUpLocation) : query.OrderByDescending(c => c.PickUpLocation);
                            break;
                        case "destination":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.Destination) : query.OrderByDescending(c => c.Destination);
                            break;
                        case "status":
                            query = sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status);
                            break;

                        default:
                            break;
                    }
                }
                var Records = query.Count();

                // Apply filtering based on the search parameter
                if (!string.IsNullOrEmpty(nameSearch))
                {
                    query = query.Where(c => c.Name.ToLower().Contains(nameSearch.ToLower()));
                }


                // Apply pagination
                var bookings = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                // create response
                var response = new
                {
                    TotalRecords = Records,
                    ClientPreferences = bookings
                };

                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = ex.Message
                };
            }
        }
        public async Task<ResponseManager> DeleteBooking(int id)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (booking != null)
                {
                    await _unitOfWork.Bookings.Remove(booking);
                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = "Booking deleted successfully."
                    };

                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Booking not found."
                };
            }
            catch (Exception ex)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = ex.Message
                };
            }
        }
        public async Task<ResponseManager> AddOrUpdateBooking(BookingVM bookingModel)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var DBBooking = _mapper.Map<Booking>(bookingModel);
                if (bookingModel.Id == 0)
                {
                    DBBooking.Status = BookingStatus.newBooking.ToString();
                    await _unitOfWork.Bookings.Add(DBBooking);
                }
                else
                {
                    await _unitOfWork.Bookings.Update(DBBooking);
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = "Booking transaction succeeded."
                };
            }
            catch (Exception ex)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = ex.Message
                };
            }
        }
    }
}
