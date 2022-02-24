using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Solu.Framework.Services.Account;
using Solu.Shared;
using Microsoft.AspNetCore.Http;

namespace Solu.Api.Middlewares
{
    public class AuthenticatorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
                if (context.Request.Cookies.ContainsKey("Authorization")) token = context.Request.Cookies["Authorization"];

            if (token != null)
                await ValidateToken(context, token);

            await _next(context);
        }

        private async Task ValidateToken(HttpContext context, string token)
        {
            try
            {
                var accountService = context.RequestServices.GetService(typeof(IAccountService));
                var account = await ((IAccountService)accountService).GetAccountByToken(token, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);

                context.Items["UserSession"] = new UserSessionModel()
                {
                    AccountId = account.Id,
                    Token = token
                };
            }
            catch { }
        }
    }
}
