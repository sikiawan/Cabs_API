using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Model.Models.UserModels;
using Model.Models;
using System.Data;
using Model.DataContext;
using Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly RestaurantContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;

        public UserService(RestaurantContext context,
                           UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IMailService mailService,
                           IConfiguration config)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailService = mailService;
        }

        //All Users
        public async Task<ResponseManager> GetUsers(string tenantId, int page, int pageSize, string search
            //,string[] statusFilter
            )
        {
            //string[] statusFilterArray = statusFilter.Split(',');

            var userAll = await _userManager.Users.OrderByDescending(x=> x.AddedOn).ToListAsync();
            //var statusFilterList = statusFilter.ToList();
            //if (statusFilterList != null && statusFilterList.Any())
            //{
            //    string abc = userAll.FirstOrDefault().IsActive.ToString();
            //    // Assuming IsActive is a boolean property
            //    userAll = userAll.Where(x => statusFilterList.Contains(x.IsActive.ToString().ToLower())).ToList();
            //}
            if (tenantId != "0")
            {
                userAll = userAll.Where(x => x.TenantId == Convert.ToInt32(tenantId)).ToList();
            }
            var query = userAll.AsQueryable();
            var Records = query.Count();
            //if ()
            //{
            //    query = statusFilterArray.Distinct(); //query.Where(c => c.IsActive.Contains(statusFilter));
            //}
            // Apply filtering based on the search parameter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.UserName.ToLower().Contains(search.ToLower()));
            }

            // Calculate total number of records
            var totalRecords = query.Count();

            // Calculate total number of pages
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Apply pagination
            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Include total pages in the response
            var response = new
            {
                TotalPages = totalPages,
                TotalRecords = Records,
                Users = users
            };
            return new ResponseManager
            {
                IsSuccess = true,
                Response = response
            };
        }

        //GetUserByID
        public async Task<ResponseManager> GetUserById(string id)
        {
            var userById = await _userManager.FindByIdAsync(id);
            return new ResponseManager
            {
                IsSuccess = true,
                Response = userById
            };
        }

        //Create User
        public async Task<ResponseManager> CreateUser(RegisterUser model)
        {
            if (model == null)
                throw new NullReferenceException("Data provided is NULL");

            //Is User Exist
            var userFound = await _userManager.FindByEmailAsync(model.Email);

            //-Not Exists
            if (userFound == null)
            {
                var identityUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Username,
                    LocalizedName = model.LocalizedName,
                    NormalizedUserName = model.Username.Normalize(),
                    NormalizedEmail = model.Email.Normalize(),
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    TenantId = Convert.ToInt32(model.TenantId),
                    AddedOn = DateTime.UtcNow

                };

                try
                {
                    var result = await _userManager.CreateAsync(identityUser, model.Password);

                    //Setting Roles
                    if (model.Role != null)
                    {
                        var roleCheck = await _roleManager.RoleExistsAsync(model.Role);
                        if (roleCheck != true)
                        {
                            await _userManager.AddToRoleAsync(identityUser, Convert.ToString("Guest"));
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(identityUser, Convert.ToString(model.Role));
                        }

                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(identityUser, Convert.ToString("Guest"));
                    }

                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = result
                    };
                    //}
                }

                catch (DBConcurrencyException ex)
                {
                    return new ResponseManager()
                    {
                        IsSuccess = false,
                        Response = ex.Message
                    };
                }
            }
            //- User Exist
            return new ResponseManager
            {
                IsSuccess = false
            };

        }

        //Update User
        public async Task<ResponseManager> UpdateUser(string id, UpdateUser user)
        {
            if (user != null)
            {
                var findUser = await _userManager.FindByIdAsync(id);
                if (findUser != null)
                {

                    try
                    {
                        /*context.Users.Add(findUser.)*/
                        var updateUser = new ApplicationUser
                        {
                            Id= id,
                            UserName = user.Username,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNo
                        };
                        var up =  _userManager.UpdateAsync(updateUser);
                        var updatedUser = await _userManager.FindByIdAsync(updateUser.Id);
                        var updatedUserResponse = new ApplicationUser
                        {
                            Id = id,
                            UserName = updatedUser.UserName,
                            NormalizedUserName = updatedUser.Email,
                            Email = updatedUser.Email,
                            NormalizedEmail = updatedUser.Email,
                            PhoneNumber = updatedUser.PhoneNumber,

                        };
                        return new ResponseManager
                        {
                            IsSuccess = true,
                            Response = updatedUserResponse
                        };

                    }
                    catch (Exception ex)
                    {

                        return new ResponseManager
                        {
                            IsSuccess = false,
                            Response = ex.Message
                        };
                    }
                }
                return new ResponseManager()
                {
                    IsSuccess = false,
                    Response = "User not found!"
                };

            }
            return new ResponseManager()
            {
                IsSuccess = false,
                Response = "updating property should not null!"
            };

        }

        //Delete User
        public async Task<ResponseManager> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            try
            {
                await _userManager.DeleteAsync(user);

                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = " User " + id + " removed successfully!"
                };
            }
            catch
            {
                throw;
            }
            return null;

        }

        //User Exist
        public bool IsExist(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        //Additional Creating Password Hash
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}

