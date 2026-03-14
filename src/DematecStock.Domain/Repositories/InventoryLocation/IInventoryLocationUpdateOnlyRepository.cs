namespace DematecStock.Domain.Repositories.InventoryLocation
{
    public interface IInventoryLocationUpdateOnlyRepository
    {
        Task<Entities.InventoryLocation?> GetByKey(int idLocation, int idProduct);
        void Update(Entities.InventoryLocation inventoryLocation);
    }
}
