using AutoMapper;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.WarehouseLocations.SearchLocationsByName
{
    public class SearchLocationsByNameUseCase : ISearchLocationsByNameUseCase
    {
        private readonly IWarehouseLocationsReadOnlyRepository _repository;
        private readonly IMapper _mapper;

        public SearchLocationsByNameUseCase(IWarehouseLocationsReadOnlyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ResponseLocationsJson>> Execute(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation)
        {
            var locations = await _repository.GetByLocationNameQuery(query, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation);

            if (!locations.Any())
                throw new NotFoundException("Nenhuma localização encontrada com o filtro informado.");

            return _mapper.Map<List<ResponseLocationsJson>>(locations);
        }
    }
}
