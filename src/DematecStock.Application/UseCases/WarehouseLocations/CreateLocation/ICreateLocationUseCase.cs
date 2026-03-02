using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.WarehouseLocations.CreateLocation
{
    public interface ICreateLocationUseCase
    {
        Task Execute(RequestWriteWarehouseLocationJson request);
    }
}
