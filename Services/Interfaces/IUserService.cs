using Model.Models;
using Model.Models.UserModels;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseManager> GetUsers(string tenantId, int page, int pageSize, string search
            //,string[] statusFilter
            );
        Task<ResponseManager> GetUserById(string id);
        Task<ResponseManager> CreateUser(RegisterUser model);
        Task<ResponseManager> UpdateUser(string id, UpdateUser user);
        Task<ResponseManager> DeleteUser(string id);
        public bool IsExist(string id);

    }
}
