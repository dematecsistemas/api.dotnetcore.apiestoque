using AutoMapper;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.WarehouseLocations;

namespace DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations
{
    public class GetAllLocationsUseCase : IGetAllLocationsUseCase
    {
        private readonly IWarehouseLocationsReadOnlyRepository _repository;
        private readonly IMapper _mapper;

        public GetAllLocationsUseCase(IWarehouseLocationsReadOnlyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            
        }
        public async Task<List<ResponseLocationsJson>> Execute(string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation)
        {
            var locations = await _repository.GetAllWarehouseLocations(isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation);

            var locationsResponse = _mapper.Map<List<ResponseLocationsJson>>(locations);

            return locationsResponse;
        }
    }
}
