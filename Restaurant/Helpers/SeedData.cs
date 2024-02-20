using Microsoft.AspNetCore.Identity;
using Model.DataContext;
using Model.Entity;
using Model.Models;
using CabsAPI.Authorization;
using System.Diagnostics.Metrics;
using static CabsAPI.Authorization.Permissions;
using System.Security.Claims;
using System.Runtime.InteropServices;

namespace CabsAPI.Helpers
{
    public class SeedData
    {
        public static async Task<bool> Initialize(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, RestaurantContext context)
        {
            // Adding a dummy restaurant for superAdmin
            if (context.restaurants != null && !context.restaurants.Any(x => x.Name == "SuperAdmin"))
            {
                await context.restaurants.AddAsync(new RestaurantModel { Name = "SuperAdmin", LocalizedName = "سپر", IsActive = false, IsDeleted = true });
                await context.SaveChangesAsync();
            }
            // Adding roles
            string[] roles = new string[] { "SuperAdmin", "Admin", "User"};
            foreach (string role in roles)
            {

                if (roleManager.FindByNameAsync(role).Result == null)
                {
                    var newRole = await roleManager.CreateAsync(new IdentityRole(role.Trim()));
                }
            }
            // Adding superAdmin user
            var superAdminRestaurant = context.restaurants.FirstOrDefault(x => x.Name == "SuperAdmin");
            var identityUser = new ApplicationUser
            {
                Email = "superadmin@gmail.com",
                UserName = "superadmin",
                LocalizedName = "سپر ایڈمن",
                NormalizedUserName = ("superadmin").Normalize(),
                NormalizedEmail = ("superadmin@gmail.com").Normalize(),
                PhoneNumber = "0300",
                IsActive = true,
                TenantId = superAdminRestaurant.Id,
                AddedOn = DateTime.UtcNow

            };
            if (userManager.FindByEmailAsync(identityUser.Email).Result == null)
            {
                await userManager.CreateAsync(identityUser, "P@ssw0rd");
                await userManager.AddToRoleAsync(identityUser, "SuperAdmin");
            }
            // Adding permissions
            string[] permissions = new string[] {
                "Permissions.Home.View",
                "Permissions.Booking.View",
                "Permissions.BookACab.View",
            };
            // Adding permissions to superAdmin
            var superAdminUser = await userManager.FindByEmailAsync(identityUser.Email);
            foreach (string permission in permissions)
            {
                if (context.permissions != null && !context.permissions.Any(x => x.Name.ToLower() == permission.ToLower()))
                {
                    await context.permissions.AddAsync(new Permission { Type = "permission", Name = permission });
                    await context.SaveChangesAsync();
                }

                var existingClaim = await userManager.GetClaimsAsync(superAdminUser);
                var permissionClaim = new Claim(CustomClaimTypes.Permission, permission);

                if (!existingClaim.Any(c => c.Type == permissionClaim.Type && c.Value == permissionClaim.Value))
                {
                    await userManager.AddClaimAsync(superAdminUser, permissionClaim);
                }
            }
            // Adding menuItems
            MenuItems[] menuItems = new MenuItems[] {
                new MenuItems { ItemId = 1, Name = "Preference", LocalizedName = "پریفرنس", Href = "/", Permission = "Permissions.Home.View", ParentId = 0 },
                new MenuItems { ItemId = 2, Name = "Home", LocalizedName = "ہوم", Href = "/", Permission = "Permissions.Home.View", ParentId = 1 },
                new MenuItems { ItemId = 3, Name = "Bookings", LocalizedName = "سب کیٹگری", Href = "/bookings", Permission = "Permissions.Booking.View", ParentId = 1 },
                new MenuItems { ItemId = 4, Name = "Book A Cab", LocalizedName = "بک اے کیب", Href = "/bookacab", Permission = "Permissions.BookACab.View", ParentId = 1 },
            };

            foreach (MenuItems menuItem in menuItems)
            {
                if (!context.menuItems.Any(x => x.ItemId == menuItem.ItemId))
                {
                    await context.menuItems.AddAsync(menuItem);
                }
            }
            context.SaveChanges();
            return true;
        }
    }
}
