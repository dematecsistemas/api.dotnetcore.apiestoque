using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity
{
    public class UpdateOnHandQuantityUseCase : IUpdateOnHandQuantityUseCase
    {
        private readonly IInventoryLocationUpdateOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseLocationsReadOnlyRepository _warehouseLocationRepository;
        private readonly IProductReadOnlyRepository _productRepository;

        public UpdateOnHandQuantityUseCase(
            IInventoryLocationUpdateOnlyRepository repository,
            IUnitOfWork unitOfWork,
            IWarehouseLocationsReadOnlyRepository warehouseLocationRepository,
            IProductReadOnlyRepository productRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _warehouseLocationRepository = warehouseLocationRepository;
            _productRepository = productRepository;
        }

        public async Task Execute(int idLocation, int idProduct, RequestUpdateOnHandQuantityJson request)
        {
            if (request.OnHandQuantity < 0)
                throw new ErrorOnValidationException("O saldo do endereço não pode ficar negativo.");

            if (!await _warehouseLocationRepository.ExistsById(idLocation))
                throw new NotFoundException("Localização informado não foi encontrado.");

            if (!await _productRepository.ExistsAsync(idProduct))
                throw new NotFoundException("O produto informado não foi encontrado.");

            var inventoryLocation = await _repository.GetByKey(idLocation, idProduct);

            if (inventoryLocation is null)
                throw new NotFoundException("Produto não endereçado nesta localização.");

            inventoryLocation.UpdateOnHandQuantity(request.OnHandQuantity);

            _repository.Update(inventoryLocation);
            await _unitOfWork.Commit();
        }
    }
}
