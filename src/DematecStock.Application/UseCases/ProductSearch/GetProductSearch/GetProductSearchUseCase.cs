using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.PorductSearch;

namespace DematecStock.Application.UseCases.ProductSearch.GetProductSearch
{
    public class GetProductSearchUseCase : IGetProductSearchUseCase
    {
        private readonly IProductSearchReadOnlyRepository _repository;

        public GetProductSearchUseCase(IProductSearchReadOnlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseProductSearchPagedJson> Execute(string? q, int page, int pageSize, CancellationToken ct)
        {
            page = Math.Max(1, page);
            pageSize = pageSize <= 0 ? 50 : pageSize;
            pageSize = Math.Min(pageSize, 50);

            var rows = await _repository.SearchAsync(q?.Trim(), page, pageSize, ct);

            return new ResponseProductSearchPagedJson
            {
                Page = page,
                PageSize = pageSize,
                Data = rows.Select(x => new ResponseProductSearchItemJson
                {
                    IdProduct = x.IdProduct,
                    ProductDescription = x.ProductDescription,
                    BaseUoM = x.BaseUoM,
                    IsProductInactive = x.IsProductInactive,
                    Ean13Code = x.Ean13Code,
                    GrossWeight = x.GrossWeight,
                    NetWeight = x.NetWeight,
                    Height = x.Height,
                    Width = x.Width,
                    Length = x.Length,
                    IdProductGroup = x.IdProductGroup,
                    ProductGroupDescription = x.ProductGroupDescription,
                    IdProductSubgroup = x.IdProductSubgroup,
                    ProductSubgroupDescription = x.ProductSubgroupDescription
                }).ToList()
            };
        }
    }
}