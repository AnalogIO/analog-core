using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IAccountService
    {
        User GetAccountByClaims(IEnumerable<Claim> claims);

        /// <summary>
        /// Register an account. Sends out an verification email to provided email
        /// </summary>
        /// <param name="programme">Programme Id of ITU Study programme.
        Task<UserResponse> RegisterAccountAsync(string name, string email, string password, int programme);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserRequest userDto);
        Task RequestAnonymization(User user);
        Task AnonymizeAccountAsync(string token);
        Task<bool> EmailExists(string email);
    }
}