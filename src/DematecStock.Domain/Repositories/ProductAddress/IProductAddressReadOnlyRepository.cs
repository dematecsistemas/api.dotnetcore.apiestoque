using DematecStock.Domain.DTOs;

namespace DematecStock.Domain.Repositories.ProductAddress
{
    public interface IProductAddressReadOnlyRepository
    {
        Task<LocationQueryResult> GetStoredItemsByLocation(int idLocation);
        Task<ProductLocationsQueryResult> GetStoredItems(int? idProduct, string? reference, string? ean13);
    }
}
