namespace DematecStock.Domain.Repositories.WarehouseLocations
{

    public interface IPatchWarehouseLocationRepository
    {
        void Update(Entities.WarehouseLocations location);
        Task<Entities.WarehouseLocations?> GetById(int id);
    }

}
