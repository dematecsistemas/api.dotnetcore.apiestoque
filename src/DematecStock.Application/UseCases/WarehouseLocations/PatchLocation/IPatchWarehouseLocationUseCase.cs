namespace DematecStock.Application.UseCases.WarehouseLocations.PatchLocation
{

    public interface IPatchWarehouseLocationUseCase
    {
        Task ExecuteAsync(int id, PatchWarehouseLocationInput input);
    }

}
