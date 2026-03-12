namespace DematecStock.Communication.Responses
{
    public class ResponseProductSearchPagedJson
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<ResponseProductSearchItemJson> Data { get; set; } = [];
    }
}