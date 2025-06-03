using repo_nha_hang_com_ga_BE.Models;

namespace repo_nha_hang_com_ga_BE.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailModel emailModel);
    }
}