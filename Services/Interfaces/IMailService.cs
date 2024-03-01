using Model.Models;
using Model.Models.RequestModels;

namespace Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task<ResponseManager> ContactUsAsync(ContactUsRequest contactUsRequest);
    }
}