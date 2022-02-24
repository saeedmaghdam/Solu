using Solu.Framework.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Solu.Shared
{
    public class DependencyResolver
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<ISecurity, Security>();
            services.AddScoped<IJwtManager, JwtManager>();
            services.AddSingleton<INotificationHandler, NotificationHandler>();
        }
    }
}
