using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
    public class InventoryLocationRepository : IInventoryLocationWriteOnlyRepository, IInventoryLocationUpdateOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public InventoryLocationRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(InventoryLocation inventoryLocation)
        {
            await _dbContext.InventoryLocation.AddAsync(inventoryLocation);
        }

        public async Task Delete(int idLocation, int idProduct)
        {
            var entity = await _dbContext.InventoryLocation
                .FirstOrDefaultAsync(e => e.IdLocation == idLocation && e.IdProduct == idProduct);

            if (entity is not null)
                _dbContext.InventoryLocation.Remove(entity);
        }

        public void DeleteTracked(InventoryLocation inventoryLocation)
        {
            _dbContext.InventoryLocation.Remove(inventoryLocation);
        }

        public async Task<InventoryLocation?> GetByKey(int idLocation, int idProduct)
        {
            return await _dbContext.InventoryLocation
                .FirstOrDefaultAsync(e => e.IdLocation == idLocation && e.IdProduct == idProduct);
        }

        public void Update(InventoryLocation inventoryLocation)
        {
            _dbContext.InventoryLocation.Update(inventoryLocation);
        }
    }
}
