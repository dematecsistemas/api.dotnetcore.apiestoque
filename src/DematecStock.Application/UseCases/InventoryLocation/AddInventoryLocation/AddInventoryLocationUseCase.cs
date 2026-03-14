using AutoMapper;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation
{
    public class AddInventoryLocationUseCase : IAddInventoryLocationUseCase
    {
        private readonly IInventoryLocationWriteOnlyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddInventoryLocationUseCase(
            IInventoryLocationWriteOnlyRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestAddInventoryLocationJson request)
        {
            if (request.OnHandQuantity < 0)
                throw new ErrorOnValidationException("O saldo do endereço não pode ficar negativo.");

            var inventoryLocation = _mapper.Map<Domain.Entities.InventoryLocation>(request);

            await _repository.Add(inventoryLocation);
            await _unitOfWork.Commit();
        }
    }
}
