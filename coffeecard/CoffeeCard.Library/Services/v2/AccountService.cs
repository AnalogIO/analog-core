using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Token;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services.v2
{
    public class AccountService : IAccountService
    {
        private readonly CoffeeCardContext _context;
        private readonly CoffeeCard.Library.Services.IEmailService _emailService;
        private readonly CoffeeCard.Library.Services.v2.IEmailService _emailServiceV2;
        private readonly CoffeeCard.Library.Services.IHashService _hashService;
        private readonly CoffeeCard.Library.Services.ITokenService _tokenService;
        private readonly CoffeeCard.Library.Services.v2.ITokenService _tokenServiceV2;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            CoffeeCardContext context,
            CoffeeCard.Library.Services.ITokenService tokenService,
            CoffeeCard.Library.Services.IEmailService emailService,
            CoffeeCard.Library.Services.v2.IEmailService emailServiceV2,
            CoffeeCard.Library.Services.v2.ITokenService tokenServiceV2,
            CoffeeCard.Library.Services.IHashService hashService,
            ILogger<AccountService> logger
        )
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _emailServiceV2 = emailServiceV2;
            _tokenServiceV2 = tokenServiceV2;
            _hashService = hashService;
            _logger = logger;
        }

        public async Task<User> RegisterAccountAsync(
            string name,
            string email,
            string password,
            int programme
        )
        {
            _logger.LogInformation(
                "Trying to register new user. Name: {name} Email: {email}",
                name,
                email
            );

            if (_context.Users.Any(x => x.Email == email))
            {
                _logger.LogInformation(
                    "Could not register user Name: {name}. Email:{email} already exists",
                    name,
                    email
                );
                throw new ApiException(
                    $"The email {email} is already being used by another user",
                    StatusCodes.Status409Conflict
                );
            }

            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(password + salt);

            var chosenProgramme = _context.Programmes.FirstOrDefault(x => x.Id == programme);
            if (chosenProgramme == null)
                throw new ApiException(
                    $"No programme found with the id: {programme}",
                    StatusCodes.Status400BadRequest
                );

            var user = new User
            {
                Name = EscapeName(name),
                Email = email,
                Password = hashedPassword,
                Salt = salt,
                Programme = chosenProgramme,
                UserGroup = UserGroup.Customer,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SendAccountVerificationEmail(user);

            return user;
        }

        private async Task SendAccountVerificationEmail(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token"),
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            await _emailService.SendRegistrationVerificationEmailAsync(user, verificationToken);
        }

        public async Task<User> UpdateAccountAsync(User user, UpdateUserRequest updateUserRequest)
        {
            if (updateUserRequest.Email != null)
            {
                if (_context.Users.Any(x => x.Email == updateUserRequest.Email))
                    throw new ApiException(
                        $"The email {updateUserRequest.Email} is already in use!",
                        400
                    );
                _logger.LogInformation(
                    "Changing email of user from {email} to {newEmail}",
                    user.Email,
                    updateUserRequest.Email
                );
                user.Email = updateUserRequest.Email;
            }

            if (updateUserRequest.Name != null)
            {
                _logger.LogInformation(
                    "Changing name of user from {oldName} to {newName}",
                    user.Name,
                    EscapeName(updateUserRequest.Name)
                );
                user.Name = EscapeName(updateUserRequest.Name);
            }

            if (updateUserRequest.PrivacyActivated != null)
            {
                _logger.LogInformation(
                    "Changing privacy of user from {oldPrivacy} to {newPrivacy}",
                    user.PrivacyActivated,
                    (bool)updateUserRequest.PrivacyActivated
                );
                user.PrivacyActivated = (bool)updateUserRequest.PrivacyActivated;
            }

            if (updateUserRequest.ProgrammeId != null)
            {
                var programme = _context.Programmes.FirstOrDefault(x =>
                    x.Id == updateUserRequest.ProgrammeId
                );
                if (programme == null)
                    throw new ApiException(
                        $"No programme with id {updateUserRequest.ProgrammeId} exists!",
                        400
                    );
                _logger.LogInformation(
                    "Changing programme of user from {oldProgramme} to {newProgramme}",
                    user.Programme.Id,
                    programme.Id
                );
                user.Programme = programme;
            }

            if (updateUserRequest.Password != null)
            {
                var salt = _hashService.GenerateSalt();
                var hashedPassword = _hashService.Hash(updateUserRequest.Password + salt);
                user.Salt = salt;
                user.Password = hashedPassword;
                _logger.LogInformation("User changed password");
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetAccountByClaimsAsync(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
                throw new ApiException("The token is invalid!", 401);

            var user = await GetAccountByEmailAsync(emailClaim.Value);
            if (user == null)
                throw new ApiException("The user could not be found", 400);

            return user;
        }

        public async Task RequestAnonymizationAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token"),
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            await _emailService.SendVerificationEmailForDeleteAccount(user, verificationToken);
        }

        public async Task AnonymizeAccountAsync(string token)
        {
            _logger.LogInformation("Trying to verify deletion with token: {token}", token);

            var email = _tokenService.ValidateVerificationTokenAndGetEmail(token);
            var user = await GetAccountByEmailAsync(email);

            await AnonymizeUserAsync(user);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task ResendAccountVerificationEmail(
            ResendAccountVerificationEmailRequest request
        )
        {
            var user = await _context
                .Users.Where(u => u.Email.ToLower().Equals(request.Email.ToLower()))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new EntityNotFoundException($"Email {request.Email} not found");
            }

            if (user.IsVerified)
            {
                throw new ConflictException($"Email {request.Email} is already verified");
            }

            await SendAccountVerificationEmail(user);
        }

        private async Task AnonymizeUserAsync(User user)
        {
            user.Email = string.Empty;
            user.Name = string.Empty;
            user.Password = string.Empty;
            user.Salt = string.Empty;
            user.DateUpdated = DateTime.UtcNow;
            user.PrivacyActivated = true;
            user.UserState = UserState.Deleted;
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetAccountByEmailAsync(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
                throw new ApiException("No user found with the given email", 401);

            return user;
        }

        public async Task UpdateUserGroup(UserGroup userGroup, int userId)
        {
            User user = await GetUserByIdAsync(userId);

            user.UserGroup = userGroup;

            await _context.SaveChangesAsync();
        }

        public async Task<UserSearchResponse> SearchUsers(
            String search,
            int pageNum,
            int pageLength
        )
        {
            int skip = pageNum * pageLength;

            IQueryable<User> query;
            if (string.IsNullOrEmpty(search))
            {
                query = _context.Users;
            }
            else
            {
                query = _context.Users.Where(u =>
                    EF.Functions.Like(u.Id.ToString(), $"%{search}%")
                    || EF.Functions.Like(u.Name, $"%{search}%")
                    || EF.Functions.Like(u.Email, $"%{search}%")
                );
            }

            var totalUsers = await query.CountAsync();

            if (totalUsers < skip)
            {
                throw new BadRequestException(
                    $"The value of {nameof(pageNum)} is outside of the range of total users"
                );
            }

            var usersByPage = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageLength)
                .Select(u => new SimpleUserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    UserGroup = u.UserGroup,
                    State = u.UserState,
                })
                .ToListAsync();

            return new UserSearchResponse { TotalUsers = totalUsers, Users = usersByPage };
        }

        private async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogError("No user was found by user id: {id}", id);
                throw new EntityNotFoundException($"No user was found by user id: {id}");
            }

            return user;
        }

        private static string EscapeName(string name)
        {
            return name.Trim('<', '>', '{', '}');
        }

        public async Task UpdatePriviligedUserGroups(WebhookUpdateUserGroupRequest request)
        {
            await _context
                .Users.Where(user => user.UserGroup != UserGroup.Customer)
                .ExecuteUpdateAsync(u => u.SetProperty(user => user.UserGroup, UserGroup.Customer));

            foreach (var item in request.PrivilegedUsers)
            {
                await _context
                    .Users.Where(u => u.Id == item.AccountId)
                    .ExecuteUpdateAsync(u => u.SetProperty(user => user.UserGroup, item.UserGroup));
            }
        }

        public async Task SendMagicLinkEmail(string email, LoginType loginType)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user is null)
            {
                // Should not throw error to prevent showing a malicious user if an email is already registered
                return;
            }
            var magicLinkTokenHash = await _tokenServiceV2.GenerateMagicLinkToken(user);
            await _emailServiceV2.SendMagicLink(user, magicLinkTokenHash, loginType);
        }

        public async Task<UserLoginResponse> GenerateUserLoginFromToken(
            TokenLoginRequest loginRequest
        )
        {
            // Validate token in DB
            var foundToken = await _tokenServiceV2.GetValidTokenByHashAsync(loginRequest.Token);

            // Invalidate token in DB
            foundToken.Revoked = true;
            await _context.SaveChangesAsync();

            // Generate refresh token
            var refreshToken = await _tokenServiceV2.GenerateRefreshTokenAsync(foundToken.User);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, foundToken.User!.Email),
                new Claim(ClaimTypes.Name, foundToken.User.Name),
                new Claim("UserId", foundToken.User.Id.ToString()),
                new Claim(ClaimTypes.Role, foundToken.User.UserGroup.ToString()),
            };
            // Generate JWT token with user claims
            var jwt = _tokenService.GenerateToken(claims);

            return new UserLoginResponse() { Jwt = jwt, RefreshToken = refreshToken };
        }
    }
}
