using AutoMapper;
using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.WarehouseLocations;

namespace DematecStock.Application.UseCases.WarehouseLocations.CreateLocation
{
    public class CreateLocationUseCase : ICreateLocationUseCase
    {
        private readonly IWarehouseLocationsWriteOnlyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLocationUseCase(IWarehouseLocationsWriteOnlyRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }
        public async Task Execute(RequestWriteWarehouseLocationJson request)
        {
            var warehouseLocation = _mapper.Map<Domain.Entities.WarehouseLocations>(request);

            await _repository.Add(warehouseLocation);
            await _unitOfWork.Commit();
        }
    }
}
