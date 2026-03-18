namespace DematecStock.Exception.ExceptionsBase
{
    /// <summary>
    /// Classe base abstrata para todas as exceções de domínio da aplicação DematecStock.
    /// Subclasses definem o HTTP status code e a lista de mensagens de erro retornadas ao cliente.
    /// Capturada pelo <see cref="DematecStock.Api.Filters.ExceptionFilter"/>.
    /// </summary>
    public abstract class DematecStockException : System.Exception
    {
        /// <summary>
        /// Código de status HTTP que deve ser retornado ao cliente quando esta exceção for lançada.
        /// </summary>
        public abstract int StatusCode { get; }

        /// <summary>
        /// Retorna a lista de mensagens de erro descritivas a serem incluídas no body da resposta.
        /// </summary>
        public abstract List<string> GetErrors();
    }
}
