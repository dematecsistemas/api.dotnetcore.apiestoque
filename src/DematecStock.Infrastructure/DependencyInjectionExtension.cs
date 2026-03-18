using BaseApi.Infrastructure.Repositories;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.InventoryMovement;
using DematecStock.Domain.Repositories.ProductSearch;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.ProductAddress;
using DematecStock.Domain.Repositories.Users;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Domain.Security.Cryptography;
using DematecStock.Domain.Security.Tokens;
using DematecStock.Infrastructure.DataAccess;
using DematecStock.Infrastructure.Repositories;
using DematecStock.Infrastructure.Security.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DematecStock.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddToken(services, configuration);
            AddRepositories(services);

            services.AddScoped<IPasswordEncrypter, Security.Cryptography.BCrypt>();
        }

        private static void AddToken(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpiresMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(
                config => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IWarehouseLocationsReadOnlyRepository, WarehouseLocationRepository>();
            services.AddScoped<IWarehouseLocationsWriteOnlyRepository, WarehouseLocationRepository>();
            services.AddScoped<IPatchWarehouseLocationRepository, WarehouseLocationRepository>();

            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();

            services.AddScoped<IProductAddressReadOnlyRepository, ProductAddressRepository>();
            services.AddScoped<IProductSearchReadOnlyRepository, ProductSearchRepository>();

            services.AddScoped<IInventoryLocationWriteOnlyRepository, InventoryLocationRepository>();
            services.AddScoped<IInventoryLocationUpdateOnlyRepository, InventoryLocationRepository>();

            services.AddScoped<IInventoryMovementsWriteOnlyRepository, InventoryMovementsRepository>();

            services.AddScoped<IProductReadOnlyRepository, ProductRepository>();
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");

            services.AddDbContext<DematecStockDbContext>(config => config.UseSqlServer(connectionString));

        }
    }
}
