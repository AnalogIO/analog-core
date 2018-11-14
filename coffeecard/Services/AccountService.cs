using System.Security.Claims;
using System;
using Coffeecard.Models;
using System.Linq;
using coffeecard.Services;
using coffeecard.Helpers;
using System.Collections.Generic;
using coffeecard.Models.DataTransferObjects.User;

public class AccountService : IAccountService
{
    private readonly CoffeecardContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IHashService _hashService;

    public AccountService(CoffeecardContext context, ITokenService tokenService, IEmailService emailService, IHashService hashService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _hashService = hashService;
    }

    public User GetAccountByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email);
        if (user == null) throw new ApiException("No user found with the given email", 401);
        return user;
    }

    public string Login(string username, string password, string version)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == username);
        if (user != null)
        {
            var hashedPw = _hashService.Hash(password + user.Salt);
            if (user.Password.Equals(hashedPw))
            {
                var claims = new Claim[] { new Claim(ClaimTypes.Email, username), new Claim(ClaimTypes.Name, user.Name), new Claim("UserId", user.Id.ToString()) };
                var token = _tokenService.GenerateToken(claims);
                return token;
            }
        }
        throw new ApiException("The username or password does not match", 401);
    }

    public User RegisterAccount(RegisterDTO registerDto)
    {
        if (_context.Users.Any(x => x.Email == registerDto.Email)) throw new ApiException($"The email {registerDto.Email} is already being used by another user", 400);
        var salt = _hashService.GenerateSalt();
        var hashedPassword = _hashService.Hash(registerDto.Password + salt);

        var programme = _context.Programmes.FirstOrDefault(x => x.Id == registerDto.ProgrammeId);
        if (programme == null) throw new ApiException($"No programme found with the id: {registerDto.ProgrammeId}", 400);

        var user = new User { Name = EscapeName(registerDto.Name), Email = registerDto.Email, Password = hashedPassword, Salt = salt, Programme = programme };

        _context.Users.Add(user);
        if (_context.SaveChanges() == 0) throw new ApiException($"The user could not be created - try again in a minute", 500);

        var claims = new Claim[] { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Role, "verification_token") };
        var verificationToken = _tokenService.GenerateToken(claims);

        _emailService.SendRegistrationVerificationEmail(user, verificationToken);

        return user;
    }

    public bool VerifyRegistration(string token)
    {
        var jwtToken = _tokenService.ReadToken(token);

        if (!jwtToken.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == "verification_token")) throw new ApiException($"The token is invalid!", 400);
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (emailClaim == null) throw new ApiException($"The token is invalid!", 400);
        var user = _context.Users.FirstOrDefault(x => x.Email == emailClaim.Value);
        if (user == null) throw new ApiException($"The token is invalid!", 400);
        user.IsVerified = true;
        return _context.SaveChanges() > 0;
    }

    public User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDTO userDto)
    {
        var user = GetAccountByClaims(claims);

        if (userDto.Email != null)
        {
            if (_context.Users.Any(x => x.Email == userDto.Email)) throw new ApiException($"The email {userDto.Email} is already in use!", 400);
            user.Email = userDto.Email;
        }

        if (userDto.Name != null)
        {
            user.Name = EscapeName(userDto.Name);
        }

        if (userDto.PrivacyActivated != null)
        {
            user.PrivacyActivated = (bool)userDto.PrivacyActivated;
        }

        if (userDto.ProgrammeId != null)
        {
            var programme = _context.Programmes.FirstOrDefault(x => x.Id == userDto.ProgrammeId);
            if (programme == null) throw new ApiException($"No programme with id {userDto.ProgrammeId} exists!", 400);
        }

        if(userDto.Password != null) {
            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(userDto.Password + salt);
            user.Salt = salt;
            user.Password = hashedPassword;
        }

        _context.SaveChanges();
        return user;
    }

    private string EscapeName(string name)
    {
        return name.Trim(new Char[] { '<', '>', '{', '}' });
    }

    public User GetAccountByClaims(IEnumerable<Claim> claims)
    {
        var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (emailClaim == null) throw new ApiException($"The token is invalid!", 401);
        var user = GetAccountByEmail(emailClaim.Value);
        if (user == null) throw new ApiException($"The user could not be found", 400);
        return user;
    }
}