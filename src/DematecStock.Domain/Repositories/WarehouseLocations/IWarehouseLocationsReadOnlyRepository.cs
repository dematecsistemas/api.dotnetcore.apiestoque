namespace DematecStock.Domain.Repositories.WarehouseLocations
{
    public interface IWarehouseLocationsReadOnlyRepository
    {
        Task<List<Entities.WarehouseLocations>> GetAllWarehouseLocations();
        Task<bool> ExistsById(int idLocation);
    }
}
