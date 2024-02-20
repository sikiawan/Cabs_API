
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabsAPI.Authorization;
using Model.Models.UserModels;
using Model.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabsAPI.Controllers
{
    [Route("api")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Admin")]
    [AllowAnonymous]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("Roles")]
        [Authorize(Permissions.Roles.View)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(new UserResponseManager
            {
                IsSuccess = true,
                Message = roles
            });
        }

        [HttpPost("AddRole/Role")]
        [Authorize(Permissions.Users.Create)]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                var newRole = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }
            return Ok(new UserResponseManager
            {
                IsSuccess = true,
                Message = "Role '" + roleName + "' has been added to Role Manager!"
            });
        }

        [HttpDelete("removeRole/role")]
        [Authorize(Permissions.Users.Delete)]
        public async Task<IActionResult> RemoveRole(string roleName)
        {
            if (roleName != null)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var removeRole = await _roleManager.DeleteAsync(role);
            }
            return Ok(new UserResponseManager
            {
                IsSuccess = true,
                Message = "Role '" + roleName + "' has been Removed from Role Manager!"
            });
        }

        [HttpGet("userRoles/userId")]
        [Authorize(Permissions.Roles.View)]
        public async Task<IActionResult> GetUserRolebyId(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser != null)
            {
                var roles = await _userManager.GetRolesAsync(existingUser);
                return Ok(roles);
            };
            return BadRequest("User not found!");

        }

        [HttpPost("AddUserRole/UserRole")]
        [Authorize(Permissions.Users.Create)]
        public async Task<IActionResult> AddUserRole(string userId, string userRole)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (userRole != null)
            {
                if (await _roleManager.RoleExistsAsync(userRole))
                {
                    await _userManager.AddToRoleAsync(existingUser, userRole);
                    var addedRoles = await _userManager.GetRolesAsync(existingUser);
                    return Ok(addedRoles);
                };
                return BadRequest("Role does not exits!");
            }
            return NotFound("Please fill the required fields ! ");
        }

        [HttpDelete("RemoveUserRole/userRole")]
        [Authorize(Permissions.Users.Edit)]
        public async Task<IActionResult> RemoveUserRole(string userId, string userRole)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (userRole != null)
            {
                if (await _roleManager.RoleExistsAsync(userRole))
                {
                    await _userManager.RemoveFromRoleAsync(existingUser, userRole);
                    var addedRoles = await _userManager.GetRolesAsync(existingUser);
                    return Ok(addedRoles);
                };
                return BadRequest("Role does not exits!");
            }
            return NotFound("Please fill the required fields ! ");
        }
    }
}
