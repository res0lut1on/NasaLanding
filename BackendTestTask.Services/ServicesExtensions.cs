using System.Threading.Tasks;
using BackendTestTask.Services.Services.Generic;
using BackendTestTask.Services.Services.Generic.Implementations;
using BackendTestTask.Services.Services.Generic.Interfaces;
using BackendTestTask.Services.Services.Implementations;
using BackendTestTask.Services.Services.Interfaces;
using BackendTestTask.Services.Services.SearchImplementations;
using BackendTestTask.Services.Services.SearchInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTestTask.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IMeteoriteService, MeteoriteService>()
                .AddScoped<ICustomGenericService, CustomGenericService>()
                .AddScoped<IMeteoriteSearchService, MeteoriteSearchService>()
                .AddScoped(typeof(IRepository<>), typeof(Repository<>));
                
            return services;
        }
    }
}