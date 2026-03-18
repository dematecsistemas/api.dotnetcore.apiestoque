
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DematecStock.Exception.ExceptionsBase
{
    /// <summary>
    /// Exceção lançada quando um recurso solicitado não é encontrado no banco de dados.
    /// Resulta em HTTP 404 Not Found.
    /// </summary>
    public class NotFoundException : DematecStockException
    {
        private readonly string _errors;

        /// <inheritdoc/>
        public override int StatusCode => (int)HttpStatusCode.NotFound;

        /// <summary>
        /// Inicializa a exceção com a mensagem descritiva do recurso não encontrado.
        /// </summary>
        /// <param name="mensagem">Descrição do recurso que não foi localizado.</param>
        public NotFoundException(string mensagem)
        {
            _errors = mensagem;
        }

        /// <inheritdoc/>
        public override List<string> GetErrors()
        {
            return [_errors];
        }
    }
}
