using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawsella.Application.Settings;

namespace Tawsella.Application
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistration).Assembly));
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ApplicationRegistration).Assembly));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }
    }
}
