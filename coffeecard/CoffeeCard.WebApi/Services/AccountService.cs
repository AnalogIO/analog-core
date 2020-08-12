using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.WebApi.Services
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

        public AccountService(CoffeeCardContext context, EnvironmentSettings environmentSettings, ITokenService tokenService,
            IEmailService emailService, IHashService hashService, IHttpContextAccessor httpContextAccessor, ILoginLimiter loginLimiter, LoginLimiterSettings loginLimiterSettings)
        {
            _context = context;
            _environmentSettings = environmentSettings;
            _tokenService = tokenService;
            _emailService = emailService;
            _hashService = hashService;
            _httpContextAccessor = httpContextAccessor;
            _loginLimiter = loginLimiter;
            _loginLimiterSettings = loginLimiterSettings;
        }

        public string Login(string username, string password, string version)
        {
            Log.Information($"Logging in user with username: {username} version: {version}");

            ValidateVersion(version);

            var user = _context.Users.FirstOrDefault(x => x.Email == username && x.IsVerified);
            if (user != null)
            {

                if (_loginLimiterSettings.IsEnabled && !_loginLimiter.LoginAllowed(user)) //Login limiter is only called if it is enabled in the settings
                {
                    Log.Warning("Login attempts exceeding maximum allowed for e-mail = {username} from IP = {ipaddress} ", username,
                        _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
                    throw new ApiException($"Amount of failed login attempts exceeds the allowed amount, please wait for {_loginLimiterSettings.TimeOutPeriodInMinutes} minutes before trying again", 429);
                }
                
                var hashedPw = _hashService.Hash(password + user.Salt);
                if (user.Password.Equals(hashedPw))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, username), new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserId", user.Id.ToString())
                    };
                    var token = _tokenService.GenerateToken(claims);

                    // check for incomplete purchases //TODO Fix this and reimplement
                    //_purchaseService.CheckIncompletePurchases(user);

                    _loginLimiter.ResetLoginAttemptsForUser(user);

                    return token;
                }
            }

            Log.Information("Unsuccessful login for e-mail = {username} from IP = {ipAddress} ", username,
                _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);

            throw new ApiException("The username or password does not match. Please check that your email is verified",
                401);
        }

        public User RegisterAccount(RegisterDto registerDto)
        {
            Log.Information($"Trying to register new user. Name: {registerDto.Name} Email: {registerDto.Email}");

            if (_context.Users.Any(x => x.Email == registerDto.Email))
            {
                Log.Information(
                    $"Could not register user Name: {registerDto.Name}. Email:{registerDto.Email} already exists");
                throw new ApiException($"The email {registerDto.Email} is already being used by another user", 400);
            }

            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(registerDto.Password + salt);

            //This can potentially be implemented again, but is just sat to 1 for now
            var programme = _context.Programmes.FirstOrDefault(x => x.Id == 1);
            if (programme == null) throw new ApiException("No programme found with the id: 0", 400);

            var user = new User
            {
                Name = EscapeName(registerDto.Name),
                Email = registerDto.Email,
                Password = hashedPassword,
                Salt = salt,
                Programme = programme,
                UserGroup = UserGroup.Customer
            };

            _context.Users.Add(user);
            if (_context.SaveChanges() == 0)
                throw new ApiException("The user could not be created - try again in a minute");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token")
            };
            var verificationToken = _tokenService.GenerateToken(claims);

            _emailService.SendRegistrationVerificationEmail(user, verificationToken);

            return user;
        }

        public bool VerifyRegistration(string token)
        {
            Log.Information($"Trying to verify registration with token: {token}");

            var jwtToken = _tokenService.ReadToken(token);
            if (!jwtToken.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == "verification_token"))
                throw new ApiException("The token is invalid!", 400);

            var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) throw new ApiException("The token is invalid!", 400);

            var user = _context.Users.FirstOrDefault(x => x.Email == emailClaim.Value);
            if (user == null) throw new ApiException("The token is invalid!", 400);

            user.IsVerified = true;
            return _context.SaveChanges() > 0;
        }

        public User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDto userDto)
        {
            var user = GetAccountByClaims(claims);

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
                    $"Changing privacy of user from {user.PrivacyActivated} to {(bool) userDto.PrivacyActivated}");
                user.PrivacyActivated = (bool) userDto.PrivacyActivated;
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

            _context.SaveChanges();
            return user;
        }

        public User GetAccountByClaims(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) throw new ApiException("The token is invalid!", 401);

            var user = GetAccountByEmail(emailClaim.Value);
            if (user == null) throw new ApiException("The user could not be found", 400);

            return user;
        }

        public void UpdateExperience(int userId, int exp)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null) throw new ApiException("Could not update user");

            user.Experience += exp;

            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null) throw new ApiException($"Could not find user with id {userId}", 400);

            return user;
        }

        public void ForgotPassword(string email)
        {
            var user = GetAccountWithTokensByEmail(email);
            if (user == null) throw new ApiException($"The user could not be found {email}", 400);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "verification_token")
            };
            var verificationToken = _tokenService.GenerateToken(claims);
            user.Tokens.Add(new Token(verificationToken));
            _context.SaveChanges();
            _emailService.SendVerificationEmailForLostPw(user, verificationToken);
        }

        public async Task<bool> RecoverUser(string token, string newPassword)
        {
            var tokenObj = _tokenService.ReadToken(token);
            if (tokenObj == null) return false;

            Log.Information($"User tried to recover with token {token}");
            if (!await _tokenService.ValidateToken(token)) return false;

            var user = GetAccountByClaims(tokenObj.Claims);
            if (user == null) return false;

            Log.Information($"{user.Email} tried to recover user");
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
            var user = _context.Users
                .Include(x => x.Programme)
                //.Include(x => x.Statistics)
                .FirstOrDefault(x => x.Email == email);
            if (user == null) throw new ApiException("No user found with the given email", 401);
            return user;
        }

        private User GetAccountWithTokensByEmail(string email)
        {
            var user = _context.Users.Include(x => x.Tokens).FirstOrDefault(x => x.Email == email);

            if (user == null) throw new ApiException("No user found with the given email", 401);
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