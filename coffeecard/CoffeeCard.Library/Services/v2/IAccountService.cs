using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IAccountService
    {
        /// <summary>
        /// Retrives a users account by receiving the claims from a jwt token
        /// </summary>
        /// <param name="claims">A list of all claims from a jwt token
        Task<User> GetAccountByClaimsAsync(IEnumerable<Claim> claims);

        /// <summary>
        /// Register an account. Sends out an verification email to provided email
        /// </summary>
        /// <param name="name"> Name of the user
        /// <param name="email"> The email address of the account to be created 
        /// <param name="password"> Desired password for the account
        /// <param name="programme">Programme Id of ITU Study programme.
        Task<User> RegisterAccountAsync(string name, string email, string password, int programme);

        /// <summary>
        /// Updates an account with the non-null properties of the updateUserRequest
        /// </summary>
        /// <param name="user"> The user entity representing an account
        /// <param name="updateUserRequest"> The request to update the user, containing several nullable properties 
        Task<User> UpdateAccountAsync(User user, UpdateUserRequest updateUserRequest);

        /// <summary>
        /// Generates a token used for account deletion, and passes it to the email service
        /// </summary>
        /// <param name="user"> The user entity representing an account
        Task RequestAnonymizationAsync(User user);

        /// <summary>
        /// Anonymizes a users account by removing all identifyable information, effectively deleting it
        /// </summary>
        /// <param name="token"> The jwt token giving permission to delete the account
        Task AnonymizeAccountAsync(string token);

        /// <summary>
        /// Checks if an account already exists with the given email
        /// </summary>
        /// <param name="email"> An email address, represented as a string
        Task<bool> EmailExistsAsync(string email);
    }
}