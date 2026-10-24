using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITokenService
    {
        Task<string> GenerateMagicLinkToken(User user);
        Task<string> GenerateRefreshTokenAsync(User user);
        Task<Token> GetValidTokenByHashAsync(string tokenString);

        /// <summary>
        /// Stages removal of all tokens owned by the user.
        /// The caller is responsible for saving the changes.
        /// </summary>
        Task RemoveTokensByUserAsync(User user);
    }
}
