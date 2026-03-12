namespace DematecStock.Domain.DTOs
{
    public class FlatLocationWithProductsQueryResult
    {
        public int IdLocation { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public int IdProduct { get; set; }
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
        public string ProductDescription { get; set; } = string.Empty;
        public int IdProductGroup { get; set; }
        public string? ProductGroupDescription { get; set; }
        public int IdProductSubgroup { get; set; }
        public string? ProductSubgroupDescription { get; set; }
        public int? IdSupplier { get; set; }
        public string? SupplierName { get; set; }
        public Boolean IsProductInactive { get; set; }
        public int IdNcm { get; set; }
        public string? NcmNumber { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
        public string? ProductType { get; set; }
        public string? Ean13Code { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? PackQuantity { get; set; }

    }
}
