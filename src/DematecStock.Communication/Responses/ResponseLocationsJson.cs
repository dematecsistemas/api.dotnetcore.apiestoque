using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Communication.Responses
{
    public class ResponseLocationsJson
    {
        public int IdLocation { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public int Aisle { get; set; }

        public int Building { get; set; }

        public int Level { get; set; }

        public int Bin { get; set; }

        public string? IsOccupied { get; set; }

        public string? IsActive { get; set; }

        public string? IsMovementAllowed { get; set; }

        public string? IsAllowReplenishment { get; set; }

        public string? IsPickingLocation { get; set; }
    }
}
