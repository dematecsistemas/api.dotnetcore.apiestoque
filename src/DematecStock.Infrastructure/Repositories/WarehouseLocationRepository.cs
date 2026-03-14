using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
    public class WarehouseLocationRepository : IWarehouseLocationsReadOnlyRepository, IWarehouseLocationsWriteOnlyRepository, IPatchWarehouseLocationRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public WarehouseLocationRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(WarehouseLocations location)
        {
            await _dbContext.WarehouseLocations.AddAsync(location);
        }

        public async Task<List<WarehouseLocations>> GetAllWarehouseLocations()
        {
            return await _dbContext.WarehouseLocations.AsNoTracking().ToListAsync();
        }

        public async Task<WarehouseLocations?> GetById(int id)
        {
            return await _dbContext.WarehouseLocations
                .FirstOrDefaultAsync(location => location.IdLocation == id);
        }

        public async Task<bool> ExistsById(int id)
        {
            return await _dbContext.WarehouseLocations
                .AsNoTracking()
                .AnyAsync(l => l.IdLocation == id);
        }

        public void Update(WarehouseLocations location)
        {
            _dbContext.WarehouseLocations.Update(location);
        }
    }
}
