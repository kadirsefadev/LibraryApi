using LibraryApi.Models;

namespace LibraryApi.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
  
    }
}
