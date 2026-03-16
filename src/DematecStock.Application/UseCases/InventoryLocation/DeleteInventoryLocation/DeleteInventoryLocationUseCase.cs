using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation
{
    public class DeleteInventoryLocationUseCase : IDeleteInventoryLocationUseCase
    {
        private readonly IInventoryLocationWriteOnlyRepository _repository;
        private readonly IInventoryLocationUpdateOnlyRepository _updateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseLocationsReadOnlyRepository _warehouseLocationRepository;
        private readonly IProductReadOnlyRepository _productRepository;

        public DeleteInventoryLocationUseCase(
            IInventoryLocationWriteOnlyRepository repository,
            IInventoryLocationUpdateOnlyRepository updateRepository,
            IUnitOfWork unitOfWork,
            IWarehouseLocationsReadOnlyRepository warehouseLocationRepository,
            IProductReadOnlyRepository productRepository)
        {
            _repository = repository;
            _updateRepository = updateRepository;
            _unitOfWork = unitOfWork;
            _warehouseLocationRepository = warehouseLocationRepository;
            _productRepository = productRepository;
        }

        public async Task Execute(int idLocation, int idProduct)
        {
            if (!await _warehouseLocationRepository.ExistsById(idLocation))
                throw new NotFoundException("Localização informada não foi encontrado.");

            if (!await _productRepository.ExistsAsync(idProduct))
                throw new NotFoundException("O produto informado não foi encontrado.");

            var inventoryLocation = await _updateRepository.GetByKey(idLocation, idProduct);

            if (inventoryLocation is null)
                throw new NotFoundException("Produto não endereçado nesta localização.");

            if (inventoryLocation.OnHandQuantity != 0)
                throw new ErrorOnValidationException("O produto possui Saldo em sua localização atual, não é possível remover.");

            _repository.DeleteTracked(inventoryLocation);
            await _unitOfWork.Commit();
        }
    }
}
