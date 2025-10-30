using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Models.DataTransferObjects.v2.Token;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IAccountService
    {
        /// <summary>
        /// Retrives a users account by receiving the claims from a jwt token
        /// </summary>
        /// <param name="claims">A list of all claims from a jwt token</param>
        Task<User> GetAccountByClaimsAsync(IEnumerable<Claim> claims);

        /// <summary>
        /// Register an account. Sends out an verification email to provided email
        /// </summary>
        /// <param name="name">Name of the user</param>
        /// <param name="email">The email address of the account to be created</param>
        /// <param name="password">Desired password for the account</param>
        /// <param name="programme">Programme Id of ITU Study programme</param>
        Task<User> RegisterAccountAsync(string name, string email, string password, int programme);

        /// <summary>
        /// Updates an account with the non-null properties of the updateUserRequest
        /// </summary>
        /// <param name="user"> The user entity representing an account</param>
        /// <param name="updateUserRequest"> The request to update the user, containing several nullable properties</param>
        Task<User> UpdateAccountAsync(User user, UpdateUserRequest updateUserRequest);

        /// <summary>
        /// Generates a token used for account deletion, and passes it to the email service
        /// </summary>
        /// <param name="user"> The user entity representing an account</param>
        Task RequestAnonymizationAsync(User user);

        /// <summary>
        /// Anonymizes a users account by removing all identifyable information, effectively deleting it
        /// </summary>
        /// <param name="token"> The jwt token giving permission to delete the account</param>
        Task AnonymizeAccountAsync(string token);

        /// <summary>
        /// Checks if an account already exists with the given email
        /// </summary>
        /// <param name="email"> An email address, represented as a string</param>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Resend invite e-mail if user account is not already verified
        /// </summary>
        /// <param name="request">Email request</param>
        /// <exception cref="EntityNotFoundException">Email account not found</exception>
        Task ResendAccountVerificationEmail(ResendAccountVerificationEmailRequest request);

        /// <summary>
        /// Update a userGroup of a user with a provided id
        /// </summary>
        /// <param name="userGroup"> The user group that will be updated </param>
        /// <param name="id"> id of the user </param>
        Task UpdateUserGroup(UserGroup userGroup, int id);

        /// <summary>
        /// Search a user from the database
        /// </summary>
        /// <param name="search"> The search string from a search bar </param>
        /// <param name="pageNum"> The page number </param>
        /// <param name="pageLength"> The length of a page </param>
        Task<UserSearchResponse> SearchUsers(String search, int pageNum, int pageLength);

        /// <summary>
        /// Remove all existing priviliged user group assignments. Update users based on request contents
        /// </summary>
        Task UpdatePriviligedUserGroups(WebhookUpdateUserGroupRequest request);

        Task<UserLoginResponse> GenerateUserLoginFromToken(TokenLoginRequest loginRequest);

        Task SendMagicLinkEmail(string email, LoginType loginType);
    }
}
