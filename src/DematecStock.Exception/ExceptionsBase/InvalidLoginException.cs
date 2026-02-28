
using System.Net;

namespace DematecStock.Exception.ExceptionsBase
{
    public class InvalidLoginException : DematecStockException
    {

        public override int StatusCode => (int)HttpStatusCode.Unauthorized;

        public override List<string> GetErrors()
        {
            return ["Login ou senha inválidos"];
        }
    }
}
