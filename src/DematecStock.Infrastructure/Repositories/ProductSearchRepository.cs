using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.ProductSearch;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
    public class ProductSearchRepository : IProductSearchReadOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public ProductSearchRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductSearchQueryResult>> SearchAsync(string? q, int page, int pageSize, string? isProductInactive, CancellationToken ct)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var result = await _dbContext.ProductSearch
                .FromSqlInterpolated($"EXEC dbo.usp_Wms_ProductSearch {q}, {page}, {pageSize}")
                .AsNoTracking()
                .ToListAsync(ct);

            if (!string.IsNullOrWhiteSpace(isProductInactive))
                result = result.Where(x => x.IsProductInactive == (isProductInactive == "S")).ToList();

            return result;
        }
    }
}
