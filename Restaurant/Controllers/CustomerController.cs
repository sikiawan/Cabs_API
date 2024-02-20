using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModel;
using CabsAPI.Helpers;
using Services.Interfaces;

namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customer;

        public CustomerController(ICustomerService customer)
        {
            _customer = customer;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customer.GetById(id);
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
        public async Task<IActionResult> GetByPhone(string phone)
        {
            var result = await _customer.GetByPhone(phone);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }

        [HttpGet(Name = "GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            var result = await _customer.GetCustomers();
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }

        [HttpDelete(Name = "DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customer.DeleteCustomer(id);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Customer Management", "", "redrawCustomers", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Customer Management", ""));
            }
        }

        [HttpPost(Name = "AddOrUpdateCustomer")]
        public async Task<IActionResult> AddOrUpdateCustomer(CustomerVM customerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultFormat.ExistResult(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                           .Select(m => m.ErrorMessage).ToArray(), "Customer  Management"));
            var result = await _customer.AddOrUpdateCustomer(customerModel);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "Customer Management", "", "redrawCustomers", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "Customer Management", ""));
            }
        }
    }
}