using DematecStock.Domain.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace DematecStock.Infrastructure.Security.Cryptography
{
    public class BCrypt : IPasswordEncripter
    {
        public string Encrypt(string password)
        {
            string passwordHash = BC.HashPassword(password);
            return passwordHash;
        }

        public bool Verify(string password, string passwordHash)
        {
            return BC.Verify(password, passwordHash);
        }
    }
}
