using DematecStock.Domain.Repositories;

namespace DematecStock.Infrastructure.DataAccess
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly DematecStockDbContext _dbContext;

        public UnitOfWork(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
