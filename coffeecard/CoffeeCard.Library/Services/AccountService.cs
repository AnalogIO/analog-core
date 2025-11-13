using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services
{
    public class AccountService : IAccountService
    {
        private readonly EnvironmentSettings _environmentSettings;
        private readonly LoginLimiterSettings _loginLimiterSettings;
        private readonly CoffeeCardContext _context;
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly ILoginLimiter _loginLimiter;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            CoffeeCardContext context,
            EnvironmentSettings environmentSettings,
            ITokenService tokenService,
            IEmailService emailService,
            IHashService hashService,
            IHttpContextAccessor httpContextAccessor,
            ILoginLimiter loginLimiter,
            LoginLimiterSettings loginLimiterSettings,
            ILogger<AccountService> logger
        )
        {
            _context = context;
            _environmentSettings = environmentSettings;
            _tokenService = tokenService;
            _emailService = emailService;
            _hashService = hashService;
            _httpContextAccessor = httpContextAccessor;
            _loginLimiter = loginLimiter;
            _loginLimiterSettings = loginLimiterSettings;
            _logger = logger;
        }

        public string Login(string email, string password, string version)
        {
            _logger.LogInformation(
                "Logging in user with username: {username} version: {version}",
                email,
                version
            );

            ValidateVersion(version);

            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                if (user.UserState == UserState.Deleted)
                {
                    _logger.LogInformation("Login attempt with deleted user id = {id}", user.Id);
                    throw new ApiException(
                        "The username or password does not match",
                        StatusCodes.Status401Unauthorized
                    );
                }

                if (!user.IsVerified)
                {
                    _logger.LogInformation(
                        "E-mail not verified. E-mail = {username} from IP = {ipAddress} ",
                        email,
                        _httpContextAccessor.HttpContext.Connection.RemoteIpAddress
                    );
                    throw new ApiException(
                        "E-mail has not been verified",
                        StatusCodes.Status403Forbidden
                    );
                }

                if (_loginLimiterSettings.IsEnabled && !_loginLimiter.LoginAllowed(user)) //Login limiter is only called if it is enabled in the settings
                {
                    _logger.LogWarning(
                        "Login attempts exceeding maximum allowed for e-mail = {username} from IP = {ipaddress} ",
                        email,
                        _httpContextAccessor.HttpContext.Connection.RemoteIpAddress
                    );
                    throw new ApiException(
                        $"Amount of failed login attempts exceeds the allowed amount, please wait for {_loginLimiterSettings.TimeOutPeriodInSeconds / 60} minutes before trying again",
                        StatusCodes.Status429TooManyRequests
                    );
                }

                var hashedPw = _hashService.Hash(password + user.Salt);
                if (user.Password.Equals(hashedPw))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.UserGroup.ToString()),
                    };

                    var token = _tokenService.GenerateToken(claims);

                    _loginLimiter.ResetLoginAttemptsForUser(user);

                    return token;
                }
            }

            _logger.LogInformation(
                "Unsuccessful login for e-mail = {username} from IP = {ipAddress} ",
                email,
                _httpContextAccessor.HttpContext.Connection.RemoteIpAddress
            );

            throw new ApiException(
                "The username or password does not match",
                StatusCodes.Status401Unauthorized
            );
        }

        public async Task<User> RegisterAccountAsync(
            string name,
            string email,
            string password,
            int programme = 1
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

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token"),
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            await _emailService.SendRegistrationVerificationEmailAsync(user, verificationToken);

            return user;
        }

        public async Task<bool> VerifyRegistration(string token)
        {
            _logger.LogInformation("Trying to verify registration with token: {token}", token);

            var email = _tokenService.ValidateVerificationTokenAndGetEmail(token);
            var user = GetAccountByEmail(email);

            user.IsVerified = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto)
        {
            var user = GetAccountByClaims(claims);

            if (userDto.Email != null)
            {
                if (_context.Users.Any(x => x.Email == userDto.Email))
                    throw new ApiException($"The email {userDto.Email} is already in use!", 400);
                _logger.LogInformation(
                    "Changing email of user from {oldEmail} to {newEmail}",
                    user.Email,
                    userDto.Email
                );
                user.Email = userDto.Email;
            }

            if (userDto.Name != null)
            {
                _logger.LogInformation(
                    "Changing name of user from {oldName} to {newName}",
                    user.Name,
                    EscapeName(userDto.Name)
                );
                user.Name = EscapeName(userDto.Name);
            }

            if (userDto.PrivacyActivated != null)
            {
                _logger.LogInformation(
                    "Changing privacy of user from {oldPrivacy} to {newPrivacy}",
                    user.PrivacyActivated,
                    (bool)userDto.PrivacyActivated
                );
                user.PrivacyActivated = (bool)userDto.PrivacyActivated;
            }

            if (userDto.ProgrammeId != null)
            {
                var programme = _context.Programmes.FirstOrDefault(x =>
                    x.Id == userDto.ProgrammeId
                );
                if (programme == null)
                    throw new ApiException(
                        $"No programme with id {userDto.ProgrammeId} exists!",
                        400
                    );
                _logger.LogInformation(
                    "Changing programme of user from {oldProgrammeId} to {newProgrammeId}",
                    user.Programme.Id,
                    programme.Id
                );
                user.Programme = programme;
            }

            if (userDto.Password != null)
            {
                var salt = _hashService.GenerateSalt();
                var hashedPassword = _hashService.Hash(userDto.Password + salt);
                user.Salt = salt;
                user.Password = hashedPassword;
                _logger.LogInformation("User changed password");
            }

            _context.SaveChanges();
            return user;
        }

        public User GetAccountByClaims(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
                throw new ApiException("The token is invalid!", 401);

            var user = GetAccountByEmail(emailClaim.Value);
            if (user == null)
                throw new ApiException("The user could not be found", 400);

            return user;
        }

        public void UpdateExperience(int userId, int exp)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
                throw new ApiException("Could not update user");

            user.Experience += exp;

            _context.SaveChanges();
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = GetAccountWithTokensByEmail(email);
            if (user == null)
                throw new ApiException(
                    $"The user could not be found {email}",
                    StatusCodes.Status404NotFound
                );

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token"),
            };
            var verificationToken = _tokenService.GenerateToken(claims);
            user.Tokens.Add(new Token(verificationToken, TokenType.ResetPassword));
            _context.SaveChanges();
            await _emailService.SendVerificationEmailForLostPwAsync(user, verificationToken);
        }

        public async Task<bool> RecoverUserAsync(string token, string newPassword)
        {
            var tokenObj = _tokenService.ReadToken(token);
            if (tokenObj == null)
                return false;

            _logger.LogInformation("User tried to recover with token {token}", token);
            if (!await _tokenService.ValidateTokenIsUnusedAsync(token))
                return false;

            var user = GetAccountByClaims(tokenObj.Claims);
            if (user == null)
                return false;

            _logger.LogInformation("{email} tried to recover user", user.Email);
            var sha256Pw = _hashService.Hash(newPassword);
            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(sha256Pw + salt);
            user.Salt = salt;
            user.Password = hashedPassword;
            user.IsVerified = true;
            user.Tokens.Clear();
            await _context.SaveChangesAsync();
            return true;
        }

        private User GetAccountByEmail(string email)
        {
            var user = _context
                .Users.Where(u => u.Email == email)
                .Include(x => x.Programme)
                .FirstOrDefault();
            if (user == null)
                throw new ApiException("No user found with the given email", 401);

            return user;
        }

        private User GetAccountWithTokensByEmail(string email)
        {
            var user = _context.Users.Include(x => x.Tokens).FirstOrDefault(x => x.Email == email);

            if (user == null)
                throw new ApiException("No user found with the given email", 401);
            return user;
        }

        private static string EscapeName(string name)
        {
            return name.Trim('<', '>', '{', '}');
        }

        private void ValidateVersion(string version)
        {
            var regex = new Regex(@"(\d+.)(\d+.)(\d+)");
            var match = regex.Match(version);

            if (!match.Success || match.Groups.Count != 4)
                throw new ApiException("Malformed version number", 400);

            var major = int.Parse(match.Groups[1].Value.TrimEnd('.'));
            var minor = int.Parse(match.Groups[2].Value.TrimEnd('.'));

            var requiredMatch = regex.Match(_environmentSettings.MinAppVersion);
            var requiredMajor = int.Parse(requiredMatch.Groups[1].Value.TrimEnd('.'));
            var requiredMinor = int.Parse(requiredMatch.Groups[2].Value.TrimEnd('.'));

            if (requiredMajor > major || requiredMinor > minor)
                throw new ApiException("Your App is out of date - please update!", 409);
        }
    }
}
