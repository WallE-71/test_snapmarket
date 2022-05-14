using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SnapMarket.Common.Api.Attributes
{
    public class JwtAuthenticationAttribute : AuthorizeAttribute
    {
        public JwtAuthenticationAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
