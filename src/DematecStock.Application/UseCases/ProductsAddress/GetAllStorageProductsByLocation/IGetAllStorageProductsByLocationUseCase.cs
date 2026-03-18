using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation
{
    public interface IGetAllStorageProductsByLocationUseCase
    {
        Task<ResponseLocationProduct> Execute(int idLocation, string? isProductInactive);
    }
}
