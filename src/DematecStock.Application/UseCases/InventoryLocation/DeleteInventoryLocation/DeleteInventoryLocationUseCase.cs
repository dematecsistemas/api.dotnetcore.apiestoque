using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation
{
    public class DeleteInventoryLocationUseCase : IDeleteInventoryLocationUseCase
    {
        private readonly IInventoryLocationWriteOnlyRepository _repository;
        private readonly IInventoryLocationUpdateOnlyRepository _updateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteInventoryLocationUseCase(
            IInventoryLocationWriteOnlyRepository repository,
            IInventoryLocationUpdateOnlyRepository updateRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _updateRepository = updateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(int idLocation, int idProduct)
        {
            var inventoryLocation = await _updateRepository.GetByKey(idLocation, idProduct);

            if (inventoryLocation is null)
                throw new NotFoundException("Localização de estoque não encontrada.");

            if (inventoryLocation.OnHandQuantity != 0)
                throw new ErrorOnValidationException("O produto possui Saldo em sua localização atual, não é possível remover.");

            await _repository.Delete(idLocation, idProduct);
            await _unitOfWork.Commit();
        }
    }
}
