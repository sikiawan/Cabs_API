using Microsoft.AspNetCore.Http;
using Model.Models;
using Model.Models.AuthModels;
using Model.Models.UserModels;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseManager> RegisterUser(RegisterUser model);
        Task<ResponseManager> LoginUser(AuthUser model, HttpRequest Request);
        Task<ResponseManager> LoginUserById(string id, string saId);
        Task<ResponseManager> ConfirmEmail(string userId, string token);
        Task<ResponseManager> ForgetPassword(string email);
        Task<ResponseManager> ResetPassword(ResetPasswordModel model);
    }
}
