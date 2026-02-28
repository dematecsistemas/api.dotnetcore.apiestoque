using System.Net;

namespace DematecStock.Exception.ExceptionsBase
{
    public class ErrorOnValidationException : DematecStockException
    {
        private readonly List<string> _erros;

        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public ErrorOnValidationException(List<string> errorMessages)
        {
            _erros = errorMessages;
        }

        public ErrorOnValidationException(String errorMessage)
        {
            _erros!.Add(errorMessage);
        }

        public override List<string> GetErrors()
        {
            return _erros;
        }
    }
}
