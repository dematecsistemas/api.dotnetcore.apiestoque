namespace DematecStock.Domain.DTOs
{
    public class ProductLocationsQueryResult
    {
        public string? Reference { get; set; }
        public int IdProduct { get; set; }
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
        public List<ProductWithLocationsQueryResult> StorageBin { get; set; } = [];
    }
}
