using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.ProductSearch.GetProductSearch
{
    public interface IGetProductSearchUseCase
    {
        Task<ResponseProductSearchPagedJson> Execute(string? q, int page, int pageSize, string? isProductInactive, CancellationToken ct);
    }
}