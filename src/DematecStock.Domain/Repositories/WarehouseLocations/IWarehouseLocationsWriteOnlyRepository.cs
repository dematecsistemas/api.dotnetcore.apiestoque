
namespace DematecStock.Domain.Repositories.WarehouseLocations
{
    public interface IWarehouseLocationsWriteOnlyRepository
    {
        Task Add(Entities.WarehouseLocations location);
    }
}
