using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITokenService
    {
        string GenerateMagicLink(string email);
        Task<string> GenerateRefreshTokenAsync(User user);
    }
}