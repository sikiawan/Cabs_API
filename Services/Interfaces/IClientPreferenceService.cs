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
    public interface IClientPreferenceService
    {
        Task<ResponseManager> GetById(int id);
        Task<ResponseManager> GetClientPreferences(int page = 1, int pageSize = 10, string search = "");
        Task<ResponseManager> GetClientPreferenceByTenantId(int tenantId, HttpRequest Request);
        Task<ResponseManager> DeleteClientPreference(int id);
        Task<ResponseManager> AddOrUpdateClientPreference(ClientPreferenceVM clientPreferenceModel);
        Task<ResponseManager> UpdateClientPreference(int id, string? theme, string? language);
    }
}
