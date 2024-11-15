using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITokenService
    {
        string GenerateMagicLink(User user);
        Task<string> GenerateRefreshTokenAsync(User user);
        Task<Token> GetValidTokenByHashAsync(string tokenString);
    }
}