using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CabsAPI.Authorization;
using Model.Models.UserModels;
using Model.Models.RequestModels;
using Services.Interfaces;

namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;

        public UserController(IUserService userService)
        {
            _user = userService;
        }

        // GET: api/Users
        [HttpPost]
        [Authorize(Permissions.Roles.View)]
        public async Task<IActionResult> GetUsers(RequestGetUsers request) 
        {
            //string[] statusFilterArray = statusFilter.Split(',');
            var AllUser = await _user.GetUsers(request.tenant.ToString(), request.page, request.pageSize, request.search
                //, request.statusFilter
                );
            return Ok(AllUser.Response);
        }
        // GET: api/Users/5
        [HttpGet("{id}")]
        //[Authorize(Roles = "SuperAdmin, Admin, Agent")]
        [Authorize(Permissions.Roles.View)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userById = await _user.GetUserById(id);

            if (userById == null)
            {
                return NotFound("User for the $`{id}` not found!");
            }

            return Ok(userById);
        }

        // PUT: api/Users/5
        //[Authorize(Roles = "SuperAdmin, Admin")]
        [Authorize(Permissions.Users.Edit)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UpdateUser user)
        {
            if (user != null)
            {
                var updateUser = await _user.GetUserById(id);
                if(updateUser!= null) {
                    var userUpdated = await _user.UpdateUser(id, user);
                    return Ok(userUpdated);
                }
            }
            return BadRequest();
            
        }


        //// POST: api/Users
        //[Authorize(Permissions.Users.Create)]
        ////[Authorize(Permissions.Users.Create)]
        //[HttpPost]
        //public async Task<IActionResult> PostUser([FromBody] RegisterUser user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _auth.RegisterUser(user);

        //        if (result.IsSuccess)
        //            return Ok(result); // Status Code: 200 

        //        return BadRequest(result);
        //    }

        //    return BadRequest("Some properties are not valid"); // Status code: 400
        //}

        // DELETE: api/Users/5
        //[AllowAnonymous]
        //[Authorize(Roles = "SuperAdmin")]
        [Authorize(Permissions.Users.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _user.GetUserById(id);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            await _user.DeleteUser(id);
            return Content("User Deleted");
        }

        private bool UserExists(string id)
        {
            return _user.IsExist(id);
        }
    }
}
