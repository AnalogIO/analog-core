using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.User;

namespace CoffeeCard.WebApi.Services
{
    public interface IAccountService
    {
        User GetAccountByClaims(IEnumerable<Claim> claims);
        User GetUserById(int userId);
        User RegisterAccount(RegisterDto registerDto);
        string Login(string username, string password, string version);
        bool VerifyRegistration(string token);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto);
        void UpdateExperience(int userId, int exp);
        void ForgotPassword(string email);
        Task<bool> RecoverUser(string token, string newPassword);
    }
}