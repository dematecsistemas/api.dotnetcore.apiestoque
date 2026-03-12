using DematecStock.Domain.DTOs;

namespace DematecStock.Domain.Repositories.PorductSearch
{
    public interface IProductSearchReadOnlyRepository
    {
        Task<List<ProductSearchQueryResult>> SearchAsync(string? q, int page, int pageSize, CancellationToken ct);
    }
}
