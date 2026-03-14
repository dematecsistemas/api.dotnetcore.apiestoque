namespace DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation
{
    public interface IDeleteInventoryLocationUseCase
    {
        Task Execute(int idLocation, int idProduct);
    }
}
