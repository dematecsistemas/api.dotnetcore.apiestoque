using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.PorductSearch;
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

        public async Task<List<ProductSearchQueryResult>> SearchAsync(string? q, int page, int pageSize, CancellationToken ct)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // A procedure atual recebe apenas @q e @take.
            // Estratégia: busca um conjunto maior e pagina em memória no padrão do projeto.
            var take = page * pageSize;

            var result = await _dbContext.ProductSearch
                .FromSqlInterpolated($"EXEC dbo.usp_Wms_ProductSearch {q}, {take}")
                .AsNoTracking()
                .ToListAsync(ct);

            return result
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
