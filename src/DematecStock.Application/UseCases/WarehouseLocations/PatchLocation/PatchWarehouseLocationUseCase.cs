using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.WarehouseLocations.PatchLocation
{

    public class PatchWarehouseLocationUseCase : IPatchWarehouseLocationUseCase
    {
        private readonly Domain.Repositories.WarehouseLocations.IPatchWarehouseLocationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public PatchWarehouseLocationUseCase(
            Domain.Repositories.WarehouseLocations.IPatchWarehouseLocationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int id, PatchWarehouseLocationInput input)
        {
            var location = await _repository.GetById(id);

            if (location is null)
                throw new NotFoundException("Warehouse location not found");

            // PATCH = só altera o que veio
            if (input.IsOccupied is not null)
                location.ChangeIsOcupied(input.IsOccupied);

            if (input.IsActive is not null)
                location.ChangeIsActive(input.IsActive);

            if (input.IsMovementAllowed is not null)
                location.ChangeIsMovementAllowed(input.IsMovementAllowed);

            if (input.IsAllowReplenishment is not null)
                location.ChangeIsAllowReplenishment(input.IsAllowReplenishment);

            if (input.IsPickingLocation is not null)
                location.ChangeIsPickingLocation(input.IsPickingLocation);


            _repository.Update(location);
            await _unitOfWork.Commit();
        }
    }

}
