namespace DematecStock.Domain.Repositories.WarehouseLocations
{
    public interface IWarehouseLocationsReadOnlyRepository
    {
        Task<List<Entities.WarehouseLocations>> GetAllWarehouseLocations(string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation);
        Task<List<Entities.WarehouseLocations>> GetByLocationNameQuery(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation);
        Task<bool> ExistsById(int idLocation);
    }
}
