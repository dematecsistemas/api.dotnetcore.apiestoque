using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.ProductsAddress.GetStorageProductsByLocationQuery
{
    public interface IGetStorageProductsByLocationQueryUseCase
    {
        Task<List<ResponseLocationProduct>> Execute(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive);
    }
}
