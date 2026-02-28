
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DematecStock.Exception.ExceptionsBase
{
    public class NotFoundException : DematecStockException
    {
        private readonly string _errors;
        public override int StatusCode => (int)HttpStatusCode.NotFound;

        public NotFoundException(string mensagem)
        {
            _errors = mensagem;
        }

        public override List<string> GetErrors()
        {
            return [_errors];
        }
    }
}
