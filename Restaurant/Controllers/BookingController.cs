using CabsAPI.Helpers;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModel.RequestVMs;
using Model.ViewModel;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Model.Models.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _booking;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        public BookingController(IBookingService booking,  IUnitOfWork unitOfWork, IMailService mailService)
        {
            _booking = booking;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _booking.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }
        [HttpGet(Name = "GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _booking.GetAllBookings();
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> GetBookings(BookingsRequest request)
        {
            var result = await _booking.GetBookings(request.Page, request.PageSize, request.NameSearch, request.SortBy, request.SortOrder);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }

        [HttpDelete(Name = "DeleteBooking")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _booking.DeleteBooking(id);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Booking Management", "", "redrawBooking", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Booking Management", ""));
            }
        }

        [HttpPost(Name = "AddOrUpdateBooking")]
        public async Task<IActionResult> AddOrUpdateBooking(BookingVM bookingModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultFormat.ExistResult(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                           .Select(m => m.ErrorMessage).ToArray(), "Booking  Management"));
            var result = await _booking.AddOrUpdateBooking(bookingModel);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Booking Management", "", "redrawBooking", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Booking Management", ""));
            }
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultFormat.ExistResult(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                           .Select(m => m.ErrorMessage).ToArray(), "Email  Management"));
            var result = await _mailService.ContactUsAsync(request);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Email Management", "", "redrawBooking", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Email Management", ""));
            }
        }
    }
}
