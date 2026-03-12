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
        public async Task<List<ResponseLocationsJson>> Execute()
        {
            var locations = await _repository.GetAllWarehouseLocations();

            var locationsResponse = _mapper.Map<List<ResponseLocationsJson>>(locations);

            return locationsResponse;
        }
    }
}
