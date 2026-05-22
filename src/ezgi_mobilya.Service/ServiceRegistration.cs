using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ezgi_mobilya.Service.Services;

namespace ezgi_mobilya.Service
{
    public static class ServiceRegistration
    {
        public static void AddServiceLayer(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
            services.AddValidatorsFromAssembly(assembly);

            // Register Business Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IContactMessageService, ContactMessageService>();
            services.AddScoped<ISocialMediaService, SocialMediaService>();
        }
    }
}
