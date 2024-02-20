using Model.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Permission> Permission { get; }
        IRepository<ClientPreference> ClientPreference { get; }
        IRepository<RestaurantModel> Restaurants { get; }
        IRepository<MenuItems> MenuItems { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Booking> Bookings { get; }
        public Task<int> Complete();

    }
}
