using System.Net;

namespace DematecStock.Exception.ExceptionsBase
{
    /// <summary>
    /// Exceção lançada quando a validação de dados de entrada falha.
    /// Resulta em HTTP 400 Bad Request com a lista de mensagens de erro.
    /// </summary>
    public class ErrorOnValidationException : DematecStockException
    {
        private readonly List<string> _erros;

        /// <inheritdoc/>
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        /// <summary>
        /// Inicializa a exceção com uma lista de mensagens de erro de validação.
        /// </summary>
        /// <param name="errorMessages">Lista de erros de validação a serem retornados ao cliente.</param>
        public ErrorOnValidationException(List<string> errorMessages)
        {
            _erros = errorMessages;
        }

        /// <summary>
        /// Inicializa a exceção com uma única mensagem de erro de validação.
        /// </summary>
        /// <param name="errorMessage">Mensagem de erro a ser retornada ao cliente.</param>
        public ErrorOnValidationException(string errorMessage)
        {
            _erros = [errorMessage];
        }

        /// <inheritdoc/>
        public override List<string> GetErrors()
        {
            return _erros;
        }
    }
}
