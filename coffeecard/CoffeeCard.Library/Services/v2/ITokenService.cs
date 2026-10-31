using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITokenService
    {
        Task<string> GenerateMagicLinkToken(User user);
        Task<string> GenerateRefreshTokenAsync(User user);

        /// <summary>
        /// Generates a single-use token that lets the owner of <paramref name="email"/>
        /// complete an account registration. Any previous unused registration token
        /// for the same email is revoked.
        /// </summary>
        Task<string> GenerateRegistrationTokenAsync(string email);

        /// <summary>
        /// Returns the magic link token matching <paramref name="tokenString"/>,
        /// or throws if it is missing, revoked, expired, or not a magic link token.
        /// </summary>
        Task<Token> GetValidTokenByHashAsync(string tokenString);

        /// <summary>
        /// Returns the registration token matching <paramref name="tokenString"/>,
        /// or throws if it is missing, revoked, expired, or not a registration token.
        /// </summary>
        Task<Token> GetValidRegistrationTokenAsync(string tokenString);
    }
}
