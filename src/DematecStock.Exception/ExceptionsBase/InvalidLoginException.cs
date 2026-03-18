
using System.Net;

namespace DematecStock.Exception.ExceptionsBase
{
    /// <summary>
    /// Exceção lançada quando as credenciais de login são inválidas (usuário não encontrado ou senha incorreta).
    /// Resulta em HTTP 401 Unauthorized. A mensagem é intencionalmente genérica para não vazar
    /// qual campo está incorreto (prevenção de enumeração de usuários).
    /// </summary>
    public class InvalidLoginException : DematecStockException
    {
        /// <inheritdoc/>
        public override int StatusCode => (int)HttpStatusCode.Unauthorized;

        /// <inheritdoc/>
        public override List<string> GetErrors()
        {
            return ["Login ou senha inválidos"];
        }
    }
}
