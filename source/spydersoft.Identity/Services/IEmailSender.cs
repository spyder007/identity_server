using System.Threading.Tasks;

namespace spydersoft.Identity.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}