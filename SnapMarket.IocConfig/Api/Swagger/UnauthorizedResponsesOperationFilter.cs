using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace SnapMarket.IocConfig.Api.Swagger
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool includeUnauthorizedAndForbiddenResponses;
        private readonly string schemeName;
        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
        {
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this.schemeName = schemeName;
        }


        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var jwtbearerScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    //[ jwtbearerScheme ] = new string [] { }
                    [ jwtbearerScheme ] = new List<string>()
                }
            };

            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter);
            if (hasAnonymous) return;

            if (includeUnauthorizedAndForbiddenResponses)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }

            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter);
            if (!hasAuthorize) return;
        }
    }
}
