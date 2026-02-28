using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.Users;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class UserRepository : IUserUpdateOnlyRepository, IUserReadOnlyRepository
    {
        private readonly DematecStockDbContext _dbContext;

        public UserRepository(DematecStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login.Equals(username));
        }

        public void UpdatePassword(User user)
        {
            _dbContext.Users.Update(user);
        }
    }
}
