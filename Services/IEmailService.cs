using System.Threading.Tasks;

namespace UserManagementApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
