using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ezgi_mobilya.Service
{
    public static class ServiceRegistration
    {
        public static void AddServiceLayer(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
            services.AddValidatorsFromAssembly(assembly);
        }
    }
}
