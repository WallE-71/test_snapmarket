using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SnapMarket.Services.Contracts
{
    public interface ISignInOption<TUser> where TUser : class
    {
        Task<SignInResult> PhoneNumberSignInAsync(TUser user, string phoneNumber, bool lockoutOnFailure);
    }
}