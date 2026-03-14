using AutoMapper;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation
{
    public class AddInventoryLocationUseCase : IAddInventoryLocationUseCase
    {
        private readonly IInventoryLocationWriteOnlyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseLocationsReadOnlyRepository _warehouseLocationRepository;
        private readonly IProductReadOnlyRepository _productRepository;

        public AddInventoryLocationUseCase(
            IInventoryLocationWriteOnlyRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IWarehouseLocationsReadOnlyRepository warehouseLocationRepository,
            IProductReadOnlyRepository productRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _warehouseLocationRepository = warehouseLocationRepository;
            _productRepository = productRepository;
        }

        public async Task Execute(RequestAddInventoryLocationJson request)
        {
            if (request.OnHandQuantity < 0)
                throw new ErrorOnValidationException("O saldo do endereço não pode ficar negativo.");

            if (!await _warehouseLocationRepository.ExistsById(request.IdLocation))
                throw new NotFoundException("Localização informado não foi encontrado.");

            if (!await _productRepository.ExistsAsync(request.IdProduct))
                throw new NotFoundException("O produto informado não foi encontrado.");

            var inventoryLocation = _mapper.Map<Domain.Entities.InventoryLocation>(request);

            await _repository.Add(inventoryLocation);
            await _unitOfWork.Commit();
        }
    }
}
