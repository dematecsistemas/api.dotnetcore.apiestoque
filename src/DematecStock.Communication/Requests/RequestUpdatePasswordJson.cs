namespace DematecStock.Communication.Requests
{
    public class RequestUpdatePasswordJson
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
