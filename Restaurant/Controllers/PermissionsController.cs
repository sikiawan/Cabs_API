using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using CabsAPI.Authorization;
using Model.Entity;
using Model.Models.UserModels;
using System.Data;
using static CabsAPI.Authorization.Permissions;
using Model.Models;
using DataAccessLayer.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace CabsAPI.Controllers
{
    [Route("api/Permission")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    [AllowAnonymous]
    public class PermissionsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;


        public PermissionsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("AddPermission/{Role}")]
        [Authorize(Users.Create)]
        public async Task<IActionResult> AddPermissions(string Role, [FromBody] List<string> claims)
        {

            if (Role != null && claims != null && claims.Any())
            {
                var role = await _roleManager.FindByNameAsync(Role);

                if (role != null)
                {
                    foreach (var claim in claims)
                    {
                        // Check if the claim already exists for the role
                        var existingClaim = await _roleManager.GetClaimsAsync(role);
                        if (existingClaim.All(c => c.Type != CustomClaimTypes.Permission || c.Value != claim))
                        {
                            await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, claim));
                        }
                    }

                    return Ok(role);
                }
                else
                {
                    return NotFound($"Role '{Role}' not found.");
                }
            }

            return BadRequest("Role or claims not defined!");
        }
        [HttpPost("AddUserPermission/{userId}")]
        [Authorize(Users.Create)]
        public async Task<IActionResult> AddUserPermissions(string userId, [FromBody] List<string> claims)
        {

            if (userId != null && claims != null && claims.Any())
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    foreach (var claim in claims)
                    {
                        // Check if the claim already exists for the user
                        var existingClaim = await _userManager.GetClaimsAsync(user);
                        if (existingClaim.All(c => c.Type != CustomClaimTypes.Permission || c.Value != claim))
                        {
                            await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, claim));
                        }
                    }

                    return Ok("User claims added successfully!");
                }
                else
                {
                    return NotFound($"User with id '{userId}' not found.");
                }
            }
            return BadRequest("User or claims not defined!");
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetMenuItems(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

            var userRoles = await _roleManager.Roles.Where(x => roles.Contains(x.Name)).ToListAsync();

            var allRoleClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                allRoleClaims.AddRange(roleClaims);
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)

            }.Union(userClaims).Union(allRoleClaims);
            var menuItems = _unitOfWork.MenuItems.GetAllQuerable().ToList();
            if (claims.Any())
            {
                menuItems = menuItems
                    .Where(x => claims.Any(claim => x.Permission != null && claim.Value.Equals(x.Permission)))
                    .ToList();
            }
            return Ok(menuItems);
        }

    }
}
