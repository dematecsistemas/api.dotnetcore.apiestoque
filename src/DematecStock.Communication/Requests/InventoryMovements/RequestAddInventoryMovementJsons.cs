namespace DematecStock.Communication.Requests.InventoryMovement
{
    public class RequestAddInventoryMovementJsons
    {
        public int IdLocation { get; set; }
        public int IdProduct { get; set; }
        public int IdUser { get; set; }
        public string Operation { get; set; } = string.Empty;
        public char MovementDirection { get; set; } //'S' para saída, 'E' para entrada
        public decimal Quantity { get; set; }
        public string? Remarks { get; set; }
    }
}
