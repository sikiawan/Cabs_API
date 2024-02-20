using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.Models.UserModels;
using Model.ViewModel;
using Org.BouncyCastle.Asn1.Ocsp;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Services.Implementation
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthService _auth;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService auth, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auth = auth;
            _userManager = userManager;
        }

        public async Task<ResponseManager> GetById(int id)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (restaurant == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Restaurant not found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = restaurant
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

        public async Task<ResponseManager> GetRestaurants(int page, int pageSize, string search)
        {
            try
            {
                var query = await _unitOfWork.Restaurants.GetAllQuerable().OrderByDescending(x => x.Id).ToListAsync();
                var totalRecords = query.Count();

                // Apply filtering based on the search parameter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.ToLowerInvariant().Contains(search.ToLowerInvariant())).ToList();
                }

                // Calculate total number of records
                var records = query.Count();

                // Calculate total number of pages
                var totalPages = (int)Math.Ceiling((double)records / pageSize);

                // Apply pagination
                var restaurants = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Include total pages in the response
                var response = new
                {
                    TotalPages = totalPages,
                    TotalRecords = totalRecords,
                    Restaurants = restaurants
                };
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = response
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
        public async Task<ResponseManager> AddOrEditRestaurant(RestaurantVM restaurantModel)
        {
            try
            {
                var DBRestaurantModel = _mapper.Map<RestaurantModel>(restaurantModel);
                if (restaurantModel.Id == 0)
                {
                    DBRestaurantModel.CreatedDate = DateTime.UtcNow;
                    await _unitOfWork.Restaurants.Add(DBRestaurantModel);
                    RegisterUser registerUser = new RegisterUser();
                    registerUser.Username = restaurantModel.Name.Replace(" ", "");
                    registerUser.LocalizedName = restaurantModel.LocalizedName;
                    registerUser.Email = restaurantModel.Email;
                    registerUser.Password = restaurantModel.Password;
                    registerUser.PhoneNumber = "0000";
                    registerUser.Role = "Admin";
                    registerUser.TenantId = DBRestaurantModel.Id.ToString();
                    await _auth.RegisterUser(registerUser);

                }
                else
                {
                    DBRestaurantModel.ModifiedDate = DateTime.UtcNow;
                    await _unitOfWork.Restaurants.Update(DBRestaurantModel);
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = "Restaurant transaction succeeded."
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

        public async Task<ResponseManager> DeleteRestaurant(int id)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (restaurant != null)
                {
                    await _unitOfWork.Restaurants.Remove(restaurant);
                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = "Restaurant deleted successfully."
                    };

                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Restaurant not found."
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

        public async Task<ResponseManager> GetRestaurantsAsync(int offset = 0, int limit = 10)
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants
                    .GetAllQuerable()
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
                if(restaurants.Count == 0)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "No restaurant found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = restaurants
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

        public async Task<ResponseManager> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants.GetAllQuerable().ToListAsync();
                if (restaurants.Count == 0)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "No restaurant found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = restaurants
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

        public async Task<ResponseManager> GetRestaurantLogo(string userEmail, HttpRequest Request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "User not found"
                    };
                }

                var tenant = user?.TenantId;
                if (tenant == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Tenant not found."
                    };
                }

                var clientPreference = _unitOfWork.ClientPreference.GetQueryable(x => x.TenantId == tenant).FirstOrDefault();
                if (clientPreference == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Client Preference not found."
                    };
                }

                // Check if Request is available
                if (Request == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Request object is null."
                    };
                }

                string ImageSrc = string.Format("{0}://{1}{2}/Image/{3}", Request.Scheme, Request.Host, Request.PathBase, clientPreference.Logo);
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = ImageSrc
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
    }
}
