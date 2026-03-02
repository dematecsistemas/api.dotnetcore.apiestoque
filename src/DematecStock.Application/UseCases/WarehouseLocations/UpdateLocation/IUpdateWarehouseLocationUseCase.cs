using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.WarehouseLocations.UpdateLocation
{
    public interface IUpdateWarehouseLocationUseCase
    {
        Task<ResponseLocationsJson> Execute(int id, RequestUpdateWarehouseLocationJson request);
    }
}
