using DematecStock.Domain.Entities;

namespace DematecStock.Domain.Repositories.Users
{
    public interface IUserReadOnlyRepository
    {
        Task<User?> GetUserByUsername(string username);
    }
}
