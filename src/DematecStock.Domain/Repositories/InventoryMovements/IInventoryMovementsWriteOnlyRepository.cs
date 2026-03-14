namespace DematecStock.Domain.Repositories.InventoryMovement
{
    public interface IInventoryMovementsWriteOnlyRepository
    {
        Task Add(Entities.InventoryMovements inventoryMovement);
    }
}
