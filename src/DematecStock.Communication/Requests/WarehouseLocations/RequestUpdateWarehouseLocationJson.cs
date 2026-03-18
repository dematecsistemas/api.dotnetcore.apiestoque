namespace DematecStock.Communication.Requests.WarehouseLocations
{
    public class RequestUpdateWarehouseLocationJson
    {
        public string? IsOccupied { get; set; }
        public string? IsActive { get; set; }
        public string? IsMovementAllowed { get; set; }
        public string? IsAllowReplenishment { get; set; }
        public string? IsPickingLocation { get; set; }

    }
}
