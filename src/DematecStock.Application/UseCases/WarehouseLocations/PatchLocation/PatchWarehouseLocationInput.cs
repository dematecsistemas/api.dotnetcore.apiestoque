using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Application.UseCases.WarehouseLocations.PatchLocation
{

    public class PatchWarehouseLocationInput
    {
        public string? IsOccupied { get; set; }
        public string? IsActive { get; set; }
        public string? IsMovementAllowed { get; set; }
        public string? IsAllowReplenishment { get; set; }
        public string? IsPickingLocation { get; set; }
    }

}
