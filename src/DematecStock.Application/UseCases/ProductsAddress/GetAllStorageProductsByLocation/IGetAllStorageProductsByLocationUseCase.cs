using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation
{
    public interface IGetAllStorageProductsByLocationUseCase
    {
        Task<ResponseLocationProduct> Execute(int idLocation, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive);
    }
}
