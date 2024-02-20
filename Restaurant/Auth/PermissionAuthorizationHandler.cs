using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using System.Security.Claims;

namespace CabsAPI.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = configuration;
        }

        protected override async Task<ResponseManager> HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User != null)
            {
                // Get all the roles the user belongs to and check if any of the roles has the permission required

                // for the authorization to succeed.
                var user = await _userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    var claims = context.User.Claims.ToList();
                    var permissions = claims.Where(x => x.Type == CustomClaimTypes.Permission &&
                                                                x.Value == requirement.Permission)
                                                    .Select(x => x.Value);

                    if (permissions.Any())
                    {
                        context.Succeed(requirement);
                        return new ResponseManager
                        {
                            IsSuccess = true,
                            Response = "Authorized"
                        };
                    }
                }
                
            };

            return new ResponseManager
            { 
                IsSuccess = false,
                Response = "User not authenticated!, Please Login to continue!"
            };
        }
    }
}
