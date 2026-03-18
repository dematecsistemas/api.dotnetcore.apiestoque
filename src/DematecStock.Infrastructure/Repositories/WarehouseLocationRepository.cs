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

        public async Task<List<WarehouseLocations>> GetAllWarehouseLocations(string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation)
        {
            var query = _dbContext.WarehouseLocations.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(isActive)) query = query.Where(l => l.IsActive == isActive);
            if (!string.IsNullOrWhiteSpace(isMovementAllowed)) query = query.Where(l => l.IsMovementAllowed == isMovementAllowed);
            if (!string.IsNullOrWhiteSpace(isAllowReplenishment)) query = query.Where(l => l.IsAllowReplenishment == isAllowReplenishment);
            if (!string.IsNullOrWhiteSpace(isPickingLocation)) query = query.Where(l => l.IsPickingLocation == isPickingLocation);

            return await query.ToListAsync();
        }

        public async Task<List<WarehouseLocations>> GetByLocationNameQuery(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation)
        {
            var q = _dbContext.WarehouseLocations
                .AsNoTracking()
                .Where(l => l.LocationName.Contains(query));

            if (!string.IsNullOrWhiteSpace(isActive)) q = q.Where(l => l.IsActive == isActive);
            if (!string.IsNullOrWhiteSpace(isMovementAllowed)) q = q.Where(l => l.IsMovementAllowed == isMovementAllowed);
            if (!string.IsNullOrWhiteSpace(isAllowReplenishment)) q = q.Where(l => l.IsAllowReplenishment == isAllowReplenishment);
            if (!string.IsNullOrWhiteSpace(isPickingLocation)) q = q.Where(l => l.IsPickingLocation == isPickingLocation);

            return await q.ToListAsync();
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
