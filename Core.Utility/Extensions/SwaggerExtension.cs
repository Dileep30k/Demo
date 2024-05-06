using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Core.Utility.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration config)
        {
            var title = config.GetSection("SwaggerConfig").GetSection("Title").Value;
            var version = config.GetSection("SwaggerConfig").GetSection("Version").Value;
            var description = config.GetSection("SwaggerConfig").GetSection("Description").Value;

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = description
                });
            });

            return services;
        }

        public static IApplicationBuilder AddSwaggerApp(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            return app;
        }
    }
}
