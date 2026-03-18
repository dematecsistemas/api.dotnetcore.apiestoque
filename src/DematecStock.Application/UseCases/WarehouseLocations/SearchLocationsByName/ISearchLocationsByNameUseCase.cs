using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.WarehouseLocations.SearchLocationsByName
{
    public interface ISearchLocationsByNameUseCase
    {
        Task<List<ResponseLocationsJson>> Execute(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation);
    }
}
