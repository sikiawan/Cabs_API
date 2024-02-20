using Microsoft.AspNetCore.Http;
using Model.Models;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseManager> GetById(int id);
        Task<ResponseManager> GetByPhone(string phone);
        Task<ResponseManager> GetCustomers();
        Task<ResponseManager> DeleteCustomer(int id);
        Task<ResponseManager> AddOrUpdateCustomer(CustomerVM customerModel);
    }
}
