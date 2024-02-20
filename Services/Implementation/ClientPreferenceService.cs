using AutoMapper;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.ViewModel;
using SendGrid;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ClientPreferenceService : IClientPreferenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientPreferenceService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<ResponseManager> GetById(int id)
        {
            try
            {
                var clientPreference = await _unitOfWork.ClientPreference.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (clientPreference == null)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Response = "Client Preference not found."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = clientPreference
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

        public async Task<ResponseManager> GetClientPreferences(int page = 1, int pageSize = 10, string search = "")
        {
            try
            {
                var clientPreferencesDB = _unitOfWork.ClientPreference.GetAllQuerable().Include(x => x.Tenant).OrderByDescending(x => x.Id).ToList();
                List<ClientPreferenceVM> vm = _mapper.Map<List<ClientPreference>, List<ClientPreferenceVM>>((clientPreferencesDB).ToList());
                var query = vm.AsQueryable();
                var Records = query.Count();

                // Apply filtering based on the search parameter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));
                }

                // Calculate total number of records
                var totalRecords = query.Count();

                // Calculate total number of pages
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Apply pagination
                var clientPreferences = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Include total pages in the response
                var response = new
                {
                    TotalPages = totalPages,
                    TotalRecords = Records,
                    ClientPreferences = clientPreferences
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
        public async Task<ResponseManager> GetClientPreferenceByTenantId(int tenantId, HttpRequest Request)
        {
            try
            {
                var clientPreferenceDB = await _unitOfWork.ClientPreference.GetQueryable(x => x.TenantId == tenantId).FirstOrDefaultAsync();
                if(clientPreferenceDB != null)
                {
                    ClientPreferenceVM clientPreferenceVM = _mapper.Map<ClientPreference, ClientPreferenceVM>(clientPreferenceDB);
                    clientPreferenceVM.Logo = string.Format("{0}://{1}{2}/Image/{3}", Request.Scheme, Request.Host, Request.PathBase, clientPreferenceVM.Logo);

                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = clientPreferenceVM
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Client Preference not found."
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

        public async Task<ResponseManager> DeleteClientPreference(int id)
        {
            try
            {
                var clientPreference = await _unitOfWork.ClientPreference.GetQueryable(x => x.Id == id).FirstOrDefaultAsync();
                if (clientPreference != null)
                {
                    await _unitOfWork.ClientPreference.Remove(clientPreference);
                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = "Client Preference deleted successfully."
                    };

                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Client Preference not found."
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

        public async Task<ResponseManager> AddOrUpdateClientPreference(ClientPreferenceVM clientPreferenceModel)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var DBClientPreferenceModel = _mapper.Map<ClientPreference>(clientPreferenceModel);
                DBClientPreferenceModel.Tenant = null;
                if (clientPreferenceModel.Id == 0)
                {
                    if (clientPreferenceModel.Image != null)
                    {
                        //Save image to wwwroot/image
                        string fileName = Path.GetFileNameWithoutExtension(clientPreferenceModel.Image.FileName);
                        string extension = Path.GetExtension(clientPreferenceModel.Image.FileName);
                        DBClientPreferenceModel.Logo = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await clientPreferenceModel.Image.CopyToAsync(fileStream);
                        }
                    }
                    await _unitOfWork.ClientPreference.Add(DBClientPreferenceModel);
                }
                else
                {
                    await _unitOfWork.ClientPreference.Update(DBClientPreferenceModel);
                }
                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = "Client Preference transaction succeeded."
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

        public async Task<ResponseManager> UpdateClientPreference(int id, string? theme, string? language)
        {
            try
            {
                var clientPreference = await _unitOfWork.ClientPreference.Find(id);
                if(clientPreference != null)
                {
                    if(!string.IsNullOrEmpty(theme))
                    {
                        clientPreference.Theme = theme;
                    }
                    if (!string.IsNullOrEmpty(language))
                    {
                        clientPreference.Language = language;
                    }
                    await _unitOfWork.ClientPreference.Update(clientPreference);
                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Response = "Client Preference transaction succeeded."
                    };
                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Client Preference transaction failed."
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
