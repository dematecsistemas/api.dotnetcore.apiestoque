namespace DematecStock.Communication.Responses
{
    public class ResponseProductWithLocations
    {
        public required int IdLocation { get; set; }
        public required string LocationName { get; set; }
        public required int Aisle { get; set; }
        public required int Building { get; set; }
        public required int Level { get; set; }
        public required int Bin { get; set; }
        public required string IsActive { get; set; }
        public required string AllowsStockMovement { get; set; }
        public required string AllowsReplenishment { get; set; }
        public required string IsPickingLocation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? OnHandQuantity { get; set; }
    }
}
