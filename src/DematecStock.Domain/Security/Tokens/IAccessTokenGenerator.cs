using DematecStock.Domain.Entities;

namespace DematecStock.Domain.Security.Tokens
{
    public interface IAccessTokenGenerator
    {
        string Generate(User user);
    }
}
