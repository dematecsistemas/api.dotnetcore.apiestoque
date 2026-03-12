using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Application.UseCases.ProductSearch.GetProductSearch;
using DematecStock.Application.UseCases.ProductsAddress.GetAllLocationsByProducts;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Application.UseCases.WarehouseLocations.PatchLocation;
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
            services.AddScoped<IGetAllLocationsUseCase, GetAllLocationsUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
            services.AddScoped<ICreateLocationUseCase, CreateLocationUseCase>();
            services.AddScoped<IPatchWarehouseLocationUseCase, PatchWarehouseLocationUseCase>();
            services.AddScoped<IGetAllStorageProductsByLocationUseCase, GetAllStorageProductsByLocationUseCase>();
            services.AddScoped<IGetAllLocationsByProductUseCase, GetAllLocationsByProductUseCase>();
            services.AddScoped<IGetProductSearchUseCase, GetProductSearchUseCase>();
        }
    }
}
