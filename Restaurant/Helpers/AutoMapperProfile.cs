using AutoMapper;
using Model.Entity;
using Model.Models;
using Model.Models.UserModels;
using Model.ViewModel;

namespace CabsAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, ResponseManager>();
            CreateMap<RegisterUser, ResponseManager>();
            CreateMap<UpdateUser, ResponseManager>();
            CreateMap<Booking, BookingVM>();
            CreateMap<BookingVM, Booking>();

            
            CreateMap<Customer, CustomerVM>();
            CreateMap<CustomerVM, Customer>();
            //CreateMap<ClientPreference, ClientPreferenceVM>();
            CreateMap<ClientPreferenceVM, ClientPreference>();
            CreateMap<ClientPreference, ClientPreferenceVM>().ForMember(x => x.TenantName, m => m.MapFrom(u => u.Tenant != null ? u.Tenant.Name : null)).ReverseMap();
            CreateMap<RestaurantModel, RestaurantVM>();
            CreateMap<RestaurantVM, RestaurantModel>();

        }
        
    }
}
