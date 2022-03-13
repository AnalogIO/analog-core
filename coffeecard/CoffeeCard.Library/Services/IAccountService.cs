using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IAccountService
    {
        User GetAccountByClaims(IEnumerable<Claim> claims);
        Task<User> RegisterAccountAsync(RegisterDto registerDto);
        string Login(string email, string password, string version);
        bool VerifyRegistration(string token);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto);
        void UpdateExperience(int userId, int exp);
        Task ForgotPasswordAsync(string email);
        Task<bool> RecoverUserAsync(string token, string newPassword);
        Task RequestAnonymization(User user);
        void AnonymizeAccount(string token);
    }
}