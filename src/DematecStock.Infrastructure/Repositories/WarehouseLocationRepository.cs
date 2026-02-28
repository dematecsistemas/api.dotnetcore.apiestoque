using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
    public class WarehouseLocationRepository : IWarehouseLocationsReadOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public WarehouseLocationRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<WarehouseLocations>> GetAllWarehouseLocations()
        {
            return await _dbContext.WarehouseLocations.AsNoTracking().ToListAsync();
        }
    }
}
