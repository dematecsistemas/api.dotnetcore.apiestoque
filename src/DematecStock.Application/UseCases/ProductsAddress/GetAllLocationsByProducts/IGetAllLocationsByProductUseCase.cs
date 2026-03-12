using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct
{
    public interface IGetAllLocationsByProductUseCase
    {
        Task<ResponseProductLocations> Execute(int? idProduct, string? reference, string? ean13Code);

    }
    
}
