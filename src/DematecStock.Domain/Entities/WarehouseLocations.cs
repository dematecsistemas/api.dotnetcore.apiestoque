using DocumentFormat.OpenXml.Wordprocessing;
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
        public string LocationName { get; private set; } = string.Empty;

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

        public void ChangeIsOcupied(string  isOcupied) { IsOccupied = isOcupied; }
        public void ChangeIsActive(string isActive) { IsActive = isActive; }
        public void ChangeIsMovementAllowed(string isMovementAllowed) { IsMovementAllowed = isMovementAllowed; }
        public void ChangeIsAllowReplenishment(string isAllowReplenishment) { IsAllowReplenishment = isAllowReplenishment; }
        public void ChangeIsPickingLocation(string isPickingLocation) { IsPickingLocation = isPickingLocation; }

        public void UpdateLocation(
            int? aisle,
            int? building,
            int? level,
            int? bin)
        {
            if (aisle.HasValue) Aisle = aisle.Value;
            if (building.HasValue) Building = building.Value;
            if (level.HasValue) Level = level.Value;
            if (bin.HasValue) Bin = bin.Value;

            GenerateLocationName();
        }

        private void GenerateLocationName()
        {
            LocationName = $"R{Aisle}-P{Building}-N{Level}-A{Bin}";
        }
        public WarehouseLocations(int aisle, int building, int level, int bin)
        {
            Aisle = aisle;
            Building = building;
            Level = level;
            Bin = bin;

            GenerateLocationName();
        }


    }
}
