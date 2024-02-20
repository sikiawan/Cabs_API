using Microsoft.AspNetCore.Http;
using Model.Models;
using Model.ViewModel;

namespace Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<ResponseManager> GetById(int id);
        Task<ResponseManager> GetRestaurants(int page, int pageSize, string search);
        Task<ResponseManager> AddOrEditRestaurant(RestaurantVM restaurantModel);
        Task<ResponseManager> DeleteRestaurant(int id);
        Task<ResponseManager> GetRestaurantsAsync(int offset = 0, int limit = 10);
        Task<ResponseManager> GetAllRestaurants();
        Task<ResponseManager> GetRestaurantLogo(string userEmail, HttpRequest Request);
    }
}
