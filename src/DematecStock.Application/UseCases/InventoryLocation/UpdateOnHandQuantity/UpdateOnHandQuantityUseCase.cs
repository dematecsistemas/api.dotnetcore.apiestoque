using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity
{
    public class UpdateOnHandQuantityUseCase : IUpdateOnHandQuantityUseCase
    {
        private readonly IInventoryLocationUpdateOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOnHandQuantityUseCase(
            IInventoryLocationUpdateOnlyRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(int idLocation, int idProduct, RequestUpdateOnHandQuantityJson request)
        {
            if (request.OnHandQuantity < 0)
                throw new ErrorOnValidationException("O saldo do endereço não pode ficar negativo.");

            var inventoryLocation = await _repository.GetByKey(idLocation, idProduct);

            if (inventoryLocation is null)
                throw new NotFoundException("Localização de estoque não encontrada.");

            inventoryLocation.UpdateOnHandQuantity(request.OnHandQuantity);

            _repository.Update(inventoryLocation);
            await _unitOfWork.Commit();
        }
    }
}
