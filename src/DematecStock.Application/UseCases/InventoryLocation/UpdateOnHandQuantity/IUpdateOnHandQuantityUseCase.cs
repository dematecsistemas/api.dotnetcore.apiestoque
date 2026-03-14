using DematecStock.Communication.Requests.InventoryLocation;

namespace DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity
{
    public interface IUpdateOnHandQuantityUseCase
    {
        Task Execute(int idLocation, int idProduct, RequestUpdateOnHandQuantityJson request);
    }
}
