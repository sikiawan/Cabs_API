using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Model.Models;
using Model.ViewModel;
using CabsAPI.Helpers;
using Services.Interfaces;

namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ClientPreferenceController : ControllerBase
    {
        private readonly IClientPreferenceService _clientPreference;

        public ClientPreferenceController(IClientPreferenceService clientPreference)
        {
            _clientPreference = clientPreference;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _clientPreference.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }

        [HttpGet(Name = "GetClientPreferences")]
        public async Task<IActionResult> GetClientPreferences(int page = 1, int pageSize = 10, string search = "")
        {
            var result = await _clientPreference.GetClientPreferences(page, pageSize, search);
            if (result.IsSuccess)
            {
                return Ok(result.Response);
            }
            else
            {
                return BadRequest(result.Response);
            }
        }



        [HttpDelete(Name = "DeleteClientPreference")]
        public async Task<IActionResult> DeleteClientPreference(int id)
        {
            var result = await _clientPreference.DeleteClientPreference(id);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "ClientPreference Management", "", "redrawClientPreference", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "ClientPreference Management", ""));
            }
        }

        [HttpPost(Name = "AddOrUpdateClientPreference")]
        public async Task<IActionResult> AddOrUpdateClientPreference(ClientPreferenceVM clientPreferenceModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResultFormat.ExistResult(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                           .Select(m => m.ErrorMessage).ToArray(), "ClientPreference  Management"));
            var result = await _clientPreference.AddOrUpdateClientPreference(clientPreferenceModel);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "ClientPreference Management", "", "redrawClientPreference", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "ClientPreference Management", ""));
            }
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UpdateClientPreference(int id, string? theme, string? language)
        {
            var result = await _clientPreference.UpdateClientPreference(id, theme, language);
            if (result.IsSuccess)
            {
                return Ok(ResultFormat.SuccessResult(result.Response, "ClientPreference Management", "", "redrawClientPreference", 1));
            }
            else
            {
                return BadRequest(ResultFormat.ErrorResult(result.Response, "ClientPreference Management", ""));
            }
        }
    }
}