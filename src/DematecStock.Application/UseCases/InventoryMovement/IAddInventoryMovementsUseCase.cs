using DematecStock.Communication.Requests.InventoryMovement;

namespace DematecStock.Application.UseCases.InventoryMovement
{
    public interface IAddInventoryMovementsUseCase
    {
        Task Execute(RequestAddInventoryMovementJsons request);
    }
}
