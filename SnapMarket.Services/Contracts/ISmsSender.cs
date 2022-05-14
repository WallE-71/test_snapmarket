using System.Threading.Tasks;

namespace SnapMarket.Services.Contracts
{
    public interface ISmsSender
    {
        Task<string> SendAuthSmsAsync(string Code, string PhoneNumber);
    }
}
