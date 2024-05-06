using Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utility.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        public static IServiceCollection AddTokenBuilder(this IServiceCollection services)
        {
            services.AddTransient<ITokenBuilder, TokenBuilder>();
            return services;
        }
    }
}
