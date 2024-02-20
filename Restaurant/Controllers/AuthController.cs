using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Model.Models.AuthModels;
using Model.Models.UserModels;
using CabsAPI.Helpers;
using Services.Interfaces;

namespace CabsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService AuthService, IMapper mapper, IMailService mailService, IConfiguration configuration)
        {
            _auth = AuthService;
            _mapper = mapper;
            _mailService = mailService;
            _config = configuration;
        }

        // api/auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUser model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.RegisterUser(model);

                if (result.IsSuccess)
                    return Ok(ResultFormat.SuccessResult(result.Response, "User Management", "", "redrawUser", 1));

                return BadRequest(ResultFormat.ErrorResult(result.Response, "User Management", ""));
            }

            return BadRequest(ResultFormat.ErrorResult("User Transaction Failed", "User Management", ""));
        }

        // api/auth/Authenticate
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthUser model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.LoginUser(model, Request);

                if (result.IsSuccess)
                {
                    //var sub = "Detected - New login for "+model.Email;
                    //var content = "<h1>Hey, new login to your account noticed</h1><p>New login to your account at " + DateTime.Now +  "</p><strong>Your Login token : </strong><code> "+ result.Message + "</code><p>Expires :  "+DateTime.Now.AddHours(24);
                    //var mailContent = new MailRequest
                    //{
                    //    ToEmail= model.Email,
                    //    Subject =  sub,
                    //    Body= content
                    //};
                    //await _mailService.SendEmailAsync(mailContent);
                    return Ok(new ResponseManager
                    {
                        IsSuccess = true,
                        //Message = "We have sent you the Login Token to your registered Mail : "+model.Email+", Please use the token to access!"
                        Response = result.Response
                    });
                }

                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
        }
        // api/auth/AuthenticateById
        [HttpPost("AuthenticateById")]
        public async Task<IActionResult> AuthenticateById(string id, string saId = "0")
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.LoginUserById(id, saId);

                if (result.IsSuccess)
                {
                    return Ok(new ResponseManager
                    {
                        IsSuccess = true,
                        Response = result.Response
                    });
                }

                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
        }

        // /api/auth/ConfirmEmail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _auth.ConfirmEmail(userId, token);

            if (result.IsSuccess)
            {
                return Content("Email Verified Successfully!");
            }

            return BadRequest(result);
        }

        // api/auth/ForgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _auth.ForgetPassword(email);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        // api/auth/ResetPassword
        [HttpPost("ResetPassword")]
        //[Description("This Method will not work in Swagger, Check your Registered Mail to get the Request link, Make a request using PostMan or any API Testing Tools with the Request Link")]
        public async Task<IActionResult> ResetPassword([FromQuery]ResetPasswordModel model)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _auth.ResetPassword(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }


    }
}
