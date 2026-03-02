using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations
{
    public interface IGetAllLocationsUseCase
    {
        Task<List<ResponseLocationsJson>> Execute();
    }

}
