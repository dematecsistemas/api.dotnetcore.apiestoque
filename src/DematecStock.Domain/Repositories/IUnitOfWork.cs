namespace DematecStock.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
