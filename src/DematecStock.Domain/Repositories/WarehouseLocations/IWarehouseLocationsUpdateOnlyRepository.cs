namespace DematecStock.Domain.Repositories.WarehouseLocations
{

    public interface IWarehouseLocationsUpdateOnlyRepository
    {
        void Update(Entities.WarehouseLocations location);
        Task<Entities.WarehouseLocations?> GetById(int id);
    }

}
