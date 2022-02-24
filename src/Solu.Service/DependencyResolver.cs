using Solu.Framework.Services.Account;
using Solu.Framework.Services.Object;
using Solu.Service.Account;
using Solu.Service.Object;
using Microsoft.Extensions.DependencyInjection;

namespace Solu.Service
{
    public class DependencyResolver
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IObjectService, ObjectService>();
        }
    }
}
