using AutoMapper;
using DematecStock.Communication.Requests.InventoryMovement;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.InventoryMovement;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryMovement
{
    public class AddInventoryMovementsUseCase : IAddInventoryMovementsUseCase
    {
        private readonly IInventoryMovementsWriteOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWarehouseLocationsReadOnlyRepository _warehouseLocationRepository;
        private readonly IProductReadOnlyRepository _productRepository;
        private readonly IInventoryLocationUpdateOnlyRepository _inventoryLocationRepository;

        public AddInventoryMovementsUseCase(
            IInventoryMovementsWriteOnlyRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IWarehouseLocationsReadOnlyRepository warehouseLocationRepository,
            IProductReadOnlyRepository productRepository,
            IInventoryLocationUpdateOnlyRepository inventoryLocationRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _warehouseLocationRepository = warehouseLocationRepository;
            _productRepository = productRepository;
            _inventoryLocationRepository = inventoryLocationRepository;
        }

        public async Task Execute(RequestAddInventoryMovementJsons request)
        {
            var errors = new List<string>();

            if (request.Quantity <= 0)
                errors.Add("A quantidade do movimento deve ser maior que zero.");

            if (request.MovementDirection is not ('E' or 'S'))
                errors.Add("A direção do movimento deve ser 'E' para entrada ou 'S' para saída.");

            if (string.IsNullOrWhiteSpace(request.Operation))
                errors.Add("A operação deve ser informada.");

            if (request.IdProduct <= 0)
                errors.Add("O produto informado é inválido.");

            if (request.IdLocation <= 0)
                errors.Add("A localização informada é inválida.");

            if (request.IdUser <= 0)
                errors.Add("O usuário informado é inválido.");

            if (errors.Count > 0)
                throw new ErrorOnValidationException(errors);

            if (!await _warehouseLocationRepository.ExistsById(request.IdLocation))
                throw new NotFoundException("Localização informada não foi encontrado.");

            if (!await _productRepository.ExistsAsync(request.IdProduct))
                throw new NotFoundException("O produto informado não foi encontrado.");

            var inventoryLocation = await _inventoryLocationRepository.GetByKey(request.IdLocation, request.IdProduct);

            if (inventoryLocation is null)
                throw new NotFoundException("O produto não está endereçado neste local.");

            var newBalance = request.MovementDirection == 'E'
                ? inventoryLocation.OnHandQuantity + request.Quantity
                : inventoryLocation.OnHandQuantity - request.Quantity;

            if (newBalance < 0)
                throw new ErrorOnValidationException(
                    $"O endereço só possui {inventoryLocation.OnHandQuantity} unidade(s) disponível(is).");

            inventoryLocation.UpdateOnHandQuantity(newBalance);
            _inventoryLocationRepository.Update(inventoryLocation);

            var inventoryMovement = _mapper.Map<Domain.Entities.InventoryMovements>(request);
            inventoryMovement.MovementDate = DateTimeOffset.UtcNow;

            await _repository.Add(inventoryMovement);
            await _unitOfWork.Commit();
        }
    }
}
