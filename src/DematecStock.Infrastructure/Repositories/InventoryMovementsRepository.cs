using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.InventoryMovement;
using DematecStock.Infrastructure.DataAccess;

namespace DematecStock.Infrastructure.Repositories
{
    public class InventoryMovementsRepository :IInventoryMovementsWriteOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public InventoryMovementsRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(InventoryMovements inventoryMovement)
        {
            await _dbContext.InventoryMovements.AddAsync(inventoryMovement);
        } 
    }
}
