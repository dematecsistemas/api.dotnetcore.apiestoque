namespace DematecStock.Domain.DTOs
{
    public class ProductSearchQueryResult
    {
        public int IdProduct { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string BaseUoM { get; set; } = string.Empty;
        public bool IsProductInactive { get; set; } 
        public string? Ean13Code { get; set; } = string.Empty;
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? IdProductGroup { get; set; }
        public string ProductGroupDescription { get; set; } = string.Empty;
        public int? IdProductSubgroup { get; set; }
        public string ProductSubgroupDescription { get; set; } = string.Empty;

    }
}
