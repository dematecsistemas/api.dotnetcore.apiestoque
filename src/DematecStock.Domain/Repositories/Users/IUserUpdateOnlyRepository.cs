using DematecStock.Domain.Entities;

namespace DematecStock.Domain.Repositories.Users
{
    public interface IUserUpdateOnlyRepository
    {
        Task<User?> GetUserById(int userId);
        void UpdatePassword(User user);
    }
}
