using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Models;
using CoffeeCard.Common.Models.DataTransferObjects.User;

namespace CoffeeCard.Library.Services
{
    public interface IAccountService
    {
        User GetAccountByClaims(IEnumerable<Claim> claims);
        User RegisterAccount(RegisterDto registerDto);
        string Login(string username, string password, string version);
        bool VerifyRegistration(string token);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto);
        void UpdateExperience(int userId, int exp);
        void ForgotPassword(string email);
        Task<bool> RecoverUser(string token, string newPassword);
    }
}