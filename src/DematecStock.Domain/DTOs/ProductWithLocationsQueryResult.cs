namespace DematecStock.Domain.DTOs
{
    public class ProductWithLocationsQueryResult
    {
        public int IdLocation { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal? OnHandQuantity { get; set; }
        public int Aisle { get; set; }
        public int Building { get; set; }
        public int Level { get; set; }
        public int Bin { get; set; }
        public string IsActive { get; set; } = string.Empty;
        public string AllowsStockMovement { get; set; } = string.Empty;
        public string AllowsReplenishment { get; set; } = string.Empty;
        public string IsPickingLocation { get; set; } = string.Empty;
    }
}
