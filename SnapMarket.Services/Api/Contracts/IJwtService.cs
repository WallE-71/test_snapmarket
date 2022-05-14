using System;
using System.Threading.Tasks;
using SnapMarket.Entities.Identity;
using SnapMarket.ViewModels.Api.Users;

namespace SnapMarket.Services.Api.Contract
{
    public interface IJwtService
    {
        AuthenticationDto GenerateTokenAsync(User User);
    }
}
