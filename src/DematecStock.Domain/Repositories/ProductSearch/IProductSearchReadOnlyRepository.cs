using DematecStock.Domain.DTOs;

namespace DematecStock.Domain.Repositories.ProductSearch
{
    public interface IProductSearchReadOnlyRepository
    {
        Task<List<ProductSearchQueryResult>> SearchAsync(string? q, int page, int pageSize, string? isProductInactive, CancellationToken ct);
    }
}
