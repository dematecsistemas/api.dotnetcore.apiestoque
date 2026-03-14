using DematecStock.Communication.Requests.InventoryLocation;

namespace DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation
{
    public interface IAddInventoryLocationUseCase
    {
        Task Execute(RequestAddInventoryLocationJson request);
    }
}
