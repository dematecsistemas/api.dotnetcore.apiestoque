using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation;
using DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation;
using DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity;
using DematecStock.Application.UseCases.InventoryMovement;
using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Application.UseCases.ProductSearch.GetProductSearch;
using DematecStock.Application.UseCases.ProductsAddress.GetAllLocationsByProducts;
using DematecStock.Application.UseCases.ProductsAddress.GetStorageProductsByLocationQuery;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Application.UseCases.WarehouseLocations.PatchLocation;
using DematecStock.Application.UseCases.WarehouseLocations.SearchLocationsByName;
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
            services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapping>());
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IGetAllLocationsUseCase, GetAllLocationsUseCase>();
            services.AddScoped<ISearchLocationsByNameUseCase, SearchLocationsByNameUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
            services.AddScoped<ICreateLocationUseCase, CreateLocationUseCase>();
            services.AddScoped<IPatchWarehouseLocationUseCase, PatchWarehouseLocationUseCase>();
            services.AddScoped<IGetAllStorageProductsByLocationUseCase, GetAllStorageProductsByLocationUseCase>();
            services.AddScoped<IGetStorageProductsByLocationQueryUseCase, GetStorageProductsByLocationQueryUseCase>();
            services.AddScoped<IGetAllLocationsByProductUseCase, GetAllLocationsByProductUseCase>();
            services.AddScoped<IGetProductSearchUseCase, GetProductSearchUseCase>();
            services.AddScoped<IAddInventoryLocationUseCase, AddInventoryLocationUseCase>();
            services.AddScoped<IDeleteInventoryLocationUseCase, DeleteInventoryLocationUseCase>();
            services.AddScoped<IUpdateOnHandQuantityUseCase, UpdateOnHandQuantityUseCase>();
            services.AddScoped<IAddInventoryMovementsUseCase, AddInventoryMovementsUseCase>();

        }
    }
}
