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

        /// <summary>
        /// Register an account. Sends out an verification email to provided email
        /// </summary>
        /// <param name="programme">Programme Id of ITU Study programme. When omitted, the legacy default (1) is used, falling back to the first active programme if 1 is inactive. An explicitly provided programme must be active.</param>
        /// <param name="name">Username, to be created</param>
        /// <param name="password">Un-hashed password</param>
        /// <param name="email">Email of the user</param>
        Task<User> RegisterAccountAsync(
            string name,
            string email,
            string password,
            int? programme = null
        );
        string Login(string email, string password, string version);
        Task<bool> VerifyRegistration(string token);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto);
        void UpdateExperience(int userId, int exp);
        Task ForgotPasswordAsync(string email);
        Task<bool> RecoverUserAsync(string token, string newPassword);
    }
}
