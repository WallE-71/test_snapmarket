using System;

namespace SnapMarket.ViewModels.Api.Users
{
    public class AuthenticationDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
