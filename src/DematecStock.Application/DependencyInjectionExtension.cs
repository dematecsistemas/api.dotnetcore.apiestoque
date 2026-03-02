using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using Microsoft.Extensions.DependencyInjection;

namespace DematecStock.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddAutoMapper(services);
            AddUseCases(services);
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapping));
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IGetAllLocationsUseCase, GetAllLocaticionsUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
            services.AddScoped<ICreateLocationUseCase, CreateLocationUseCase>();
        }
    }
}
