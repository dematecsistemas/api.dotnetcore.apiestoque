namespace DematecStock.Communication.Requests.InventoryLocation
{
    public class RequestAddInventoryLocationJson
    {
        public int IdLocation { get; set; }
        public int IdProduct { get; set; }
        public string Reference { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal OnHandQuantity { get; set; }
    }
}
