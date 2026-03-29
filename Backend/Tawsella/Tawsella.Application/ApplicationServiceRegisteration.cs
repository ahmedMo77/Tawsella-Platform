using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application
{
    public static class ApplicationServiceRegisteration
    {
        public static IServiceCollection AddApplicationServcie(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegisteration).Assembly));
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ApplicationServiceRegisteration).Assembly));
            return services;
        }

    }
}
