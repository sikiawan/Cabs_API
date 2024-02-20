using DataAccessLayer.Interfaces;
using Model.DataContext;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;

        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
            Permission = new Repository<Permission>(_context);
            ClientPreference = new Repository<ClientPreference>(_context);
            Restaurants = new Repository<RestaurantModel>(_context);
            MenuItems = new Repository<MenuItems>(_context);
            Customers = new Repository<Customer>(_context);
            Bookings = new Repository<Booking>(_context);
        }

        public IRepository<Permission> Permission { get; }
        public IRepository<ClientPreference> ClientPreference { get; }
        public IRepository<RestaurantModel> Restaurants { get; }
        public IRepository<MenuItems> MenuItems { get; }
        public IRepository<Customer> Customers { get; }
        public IRepository<Booking> Bookings { get; }


        public async Task<int> Complete()
        {
            int result = await _context.SaveChangesAsync();
            return result;
        }
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync(); ;
        }


    }
}
