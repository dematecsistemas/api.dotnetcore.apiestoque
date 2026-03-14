namespace DematecStock.Domain.Repositories.InventoryLocation
{
    public interface IInventoryLocationWriteOnlyRepository
    {
        Task Add(Entities.InventoryLocation inventoryLocation);
        Task Delete(int idLocation, int idProduct);
        void DeleteTracked(Entities.InventoryLocation inventoryLocation);
    }
}
