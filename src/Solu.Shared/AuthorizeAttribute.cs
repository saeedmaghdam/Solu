using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Solu.Shared
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Items["UserSession"];
            if (context.HttpContext.Request.Path.Value == "/api/session" & context.HttpContext.Request.Method == HttpMethods.Delete & session == null)
            {
                context.Result = new OkResult();
                return;

            }

            if (session == null)
            {
                // not logged in
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
