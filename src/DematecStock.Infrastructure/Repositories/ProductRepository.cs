using DematecStock.Domain.Repositories.Product;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
    public class ProductRepository : IProductReadOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public ProductRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(int idProduct)
        {
            var count = await _dbContext.Database
                .SqlQuery<int>($"SELECT CAST(COUNT(1) AS INT) AS Value FROM Produtos WHERE CodProduto = {idProduct}")
                .FirstOrDefaultAsync();

            return count > 0;
        }
    }
}
