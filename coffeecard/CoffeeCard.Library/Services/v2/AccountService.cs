using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public class AccountService : IAccountService
    {
        private readonly CoffeeCardContext _context;
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly ITokenService _tokenService;

        public AccountService(CoffeeCardContext context, ITokenService tokenService, IEmailService emailService, IHashService hashService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _hashService = hashService;
        }

        public async Task<User> RegisterAccountAsync(string name, string email, string password, int programme)
        {
            Log.Information("Trying to register new user. Name: {name} Email: {email}", name, email);

            if (_context.Users.Any(x => x.Email == email))
            {
                Log.Information("Could not register user Name: {name}. Email:{email} already exists", name, email);
                throw new ApiException($"The email {email} is already being used by another user",
                StatusCodes.Status409Conflict);
            }

            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(password + salt);

            var chosenProgramme = _context.Programmes.FirstOrDefault(x => x.Id == programme);
            if (chosenProgramme == null)
                throw new ApiException($"No programme found with the id: {programme}", StatusCodes.Status400BadRequest);

            var user = new User
            {
                Name = EscapeName(name),
                Email = email,
                Password = hashedPassword,
                Salt = salt,
                Programme = chosenProgramme,
                UserGroup = UserGroup.Customer
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token")
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            await _emailService.SendRegistrationVerificationEmailAsync(user, verificationToken);

            return user;
        }

        public async Task<User> UpdateAccountAsync(User user, UpdateUserRequest userDto)
        {
            if (userDto.Email != null)
            {
                if (_context.Users.Any(x => x.Email == userDto.Email))
                    throw new ApiException($"The email {userDto.Email} is already in use!", 400);
                Log.Information($"Changing email of user from {user.Email} to {userDto.Email}");
                user.Email = userDto.Email;
            }

            if (userDto.Name != null)
            {
                Log.Information($"Changing name of user from {user.Name} to {EscapeName(userDto.Name)}");
                user.Name = EscapeName(userDto.Name);
            }

            if (userDto.PrivacyActivated != null)
            {
                Log.Information(
                    $"Changing privacy of user from {user.PrivacyActivated} to {(bool)userDto.PrivacyActivated}");
                user.PrivacyActivated = (bool)userDto.PrivacyActivated;
            }

            if (userDto.ProgrammeId != null)
            {
                var programme = _context.Programmes.FirstOrDefault(x => x.Id == userDto.ProgrammeId);
                if (programme == null)
                    throw new ApiException($"No programme with id {userDto.ProgrammeId} exists!", 400);
                Log.Information($"Changing programme of user from {user.Programme.Id} to {programme.Id}");
                user.Programme = programme;
            }

            if (userDto.Password != null)
            {
                var salt = _hashService.GenerateSalt();
                var hashedPassword = _hashService.Hash(userDto.Password + salt);
                user.Salt = salt;
                user.Password = hashedPassword;
                Log.Information("User changed password");
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetAccountByClaimsAsync(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) throw new ApiException("The token is invalid!", 401);

            var user = await GetAccountByEmailAsync(emailClaim.Value);
            if (user == null) throw new ApiException("The user could not be found", 400);

            return user;
        }

        public async Task RequestAnonymizationAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token")
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            await _emailService.SendVerificationEmailForDeleteAccount(user, verificationToken);
        }

        public async Task AnonymizeAccountAsync(string token)
        {
            Log.Information("Trying to verify deletion with token: {token}", token);

            var email = _tokenService.ValidateVerificationTokenAndGetEmail(token);
            var user = await GetAccountByEmailAsync(email);

            await AnonymizeUserAsync(user);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Users.AnyAsync(x => x.Email == email);
        }

        private async Task AnonymizeUserAsync(User user)
        {
            user.Email = string.Empty;
            user.Name = string.Empty;
            user.Password = string.Empty;
            user.Salt = string.Empty;
            user.DateUpdated = DateTime.Now;
            user.PrivacyActivated = true;
            user.UserState = UserState.Deleted;
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetAccountByEmailAsync(string email)
        {
            var user = await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            if (user == null) throw new ApiException("No user found with the given email", 401);

            return user;
        }

        private static string EscapeName(string name)
        {
            return name.Trim('<', '>', '{', '}');
        }
    }
}