using AutoMapper;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.WarehouseLocations;

namespace DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations
{
    public class GetAllLocaticionsUseCase : IGetAllLocationsUseCase
    {
        private readonly IWarehouseLocationsReadOnlyRepository _repository;
        private readonly IMapper _mapper;

        public GetAllLocaticionsUseCase(IWarehouseLocationsReadOnlyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            
        }
        public async Task<List<ResponseLocationsJson>> Execute()
        {
            var locations = await _repository.GetAllWarehouseLocations();

            var locationsResponse = _mapper.Map<List<ResponseLocationsJson>>(locations);

            return locationsResponse;
        }
    }
}
