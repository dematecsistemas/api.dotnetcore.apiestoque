using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Domain.Entities
{
    [Table("Estocagem")]
    public class WarehouseLocations
    {
        [Key]
        [Column("CodEst")]
        public int IdLocation { get; set; }

        [Column("DescEst")]
        public string LocationName { get; set; } = string.Empty;

        [Column("Rua")]
        public int Aisle { get; set; }

        [Column("Predio")]
        public int Building { get; set; }

        [Column("Nivel")]
        public int Level { get; set; }

        [Column("Apto")]
        public int Bin { get; set; }

        [Column("Ocupado")]
        public string? IsOccupied { get; set; }

        [Column("Ativo")]
        public string? IsActive { get; set; }

        [Column("MovEstoque")]
        public string? IsMovementAllowed { get; set; }

        [Column("PermiteRep")]
        public string? IsAllowReplenishment { get; set; }

        [Column("EstPicking")]
        public string? IsPickingLocation { get; set; }

    }
}
