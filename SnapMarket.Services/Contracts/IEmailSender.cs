using System.Threading.Tasks;

namespace SnapMarket.Services.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
