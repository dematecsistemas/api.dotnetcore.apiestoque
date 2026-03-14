namespace DematecStock.Domain.Repositories.Product
{
    public interface IProductReadOnlyRepository
    {
        Task<bool> ExistsAsync(int idProduct);
    }
}
