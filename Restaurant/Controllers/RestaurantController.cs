using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Models;
using Model.Models.UserModels;
using Model.ViewModel;
using CabsAPI.Helpers;
using Services.Interfaces;


using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurant;
        public RestaurantController(IRestaurantService restaurant)
        {
            _restaurant = restaurant;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _restaurant.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }
        [HttpGet(Name = "GetRestaurants")]
        public async Task<IActionResult> GetRestaurants(int page = 1, int pageSize = 10, string search = "")
        {
            var result = await _restaurant.GetRestaurants(page, pageSize, search);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }

        [HttpDelete(Name = "DeleteRestaurant")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurant.DeleteRestaurant(id);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Restaurant Management", "", "redrawRestaurant", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Restaurant Management", ""));
            }
        }

        [HttpPost(Name = "AddOrEditRestaurant")]
        public async Task<IActionResult> AddOrEditRestaurant(RestaurantVM restaurantModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultFormat.ExistResult(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                           .Select(m => m.ErrorMessage).ToArray(), "Restaurant  Management"));
            var result = await _restaurant.AddOrEditRestaurant(restaurantModel);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Restaurant Management", "", "redrawRestaurant", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult("Restaurant Transaction Failed", "Restaurant Management", ""));
            }
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetRestaurantsAsync(int offset = 0, int limit = 10)
        {
            var result = await _restaurant.GetRestaurantsAsync(offset, limit);
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
        [HttpGet]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var result = await _restaurant.GetAllRestaurants();
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
        [HttpGet]
        public async Task<IActionResult> GetRestaurantLogo(string userEmail)
        {
            var result = await _restaurant.GetRestaurantLogo(userEmail, Request);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }
    }
}
