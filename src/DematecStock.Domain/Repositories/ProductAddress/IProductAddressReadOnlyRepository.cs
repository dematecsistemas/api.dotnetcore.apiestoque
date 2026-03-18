using DematecStock.Domain.DTOs;

namespace DematecStock.Domain.Repositories.ProductAddress
{
    public interface IProductAddressReadOnlyRepository
    {
        Task<LocationQueryResult> GetStoredItemsByLocation(int idLocation, string? isProductInactive);
        Task<List<LocationQueryResult>> GetStoredItemsByLocationQuery(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive);
        Task<ProductLocationsQueryResult> GetStoredItems(int? idProduct, string? reference, string? ean13, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive);
    }
}
