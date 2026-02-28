namespace DematecStock.Exception.ExceptionsBase
{
    public abstract class DematecStockException : System.Exception
    {
        public abstract int StatusCode { get; }
        public abstract List<string> GetErrors();
    }
}
