using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.DataContext;
using Model.Models;
using Model.ViewModel;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<ResponseManager> GetById(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (customer == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Customer not found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = customer
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
        public async Task<ResponseManager> GetByPhone(string phone)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetQueryable(x => x.Phone == phone).FirstOrDefaultAsync();
                if (customer == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Customer not found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = customer
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
        public async Task<ResponseManager> GetCustomers()
        {
            try
            {
                var customers = await _unitOfWork.Customers.GetAll();
               return new ResponseManager
                {
                    IsSuccess = true,
                    Response = customers
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
        public async Task<ResponseManager> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (customer != null)
                {
                    await _unitOfWork.Customers.Remove(customer);
                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = "Customer deleted successfully."
                    };

                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Customer not found."
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
        public async Task<ResponseManager> AddOrUpdateCustomer(CustomerVM customerModel)
        {
            try
            {
                var DbCustomer = _mapper.Map<Customer>(customerModel);
                if (customerModel.Id == 0)
                {
                    await _unitOfWork.Customers.Add(DbCustomer);
                }
                else
                {
                    await _unitOfWork.Customers.Update(DbCustomer);
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = "Customer transaction succeeded."
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
