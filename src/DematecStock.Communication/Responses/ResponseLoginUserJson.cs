namespace DematecStock.Communication.Responses
{
    public class ResponseLoginUserJson
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
