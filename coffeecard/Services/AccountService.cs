using System.Security.Claims;
using System;
using Coffeecard.Models;
using System.Linq;
using coffeecard.Services;
using coffeecard.Helpers;

public class AccountService : IAccountService
{
    private readonly CoffeecardContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IHashService _hashService;

    public AccountService(CoffeecardContext context, ITokenService tokenService, IEmailService emailService, IHashService hashService) {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _hashService = hashService;
    }
    public User GetAccount(string token)
    {
        throw new NotImplementedException();
    }

    public int GetIdFromEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email);
        if (user == null) throw new ApiException("No user found with the given email", 401); // use better exceptions that are forwarded back to the controller and returned to the caller
        return user.Id;
    }

    public string Login(string username, string password, string version)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == username);
        if (user != null)
        {
            var hashedPw = _hashService.Hash(password + user.Salt);
            if (user.Password.Equals(hashedPw))
            {
                var claims = new Claim[] { new Claim(ClaimTypes.Email, username), new Claim(ClaimTypes.Name, user.Name) };
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
        if(programme == null) throw new ApiException($"No programme found with the id: {registerDto.ProgrammeId}", 400);

        var user = new User { Name = EscapeName(registerDto.Name), Email = registerDto.Email, Password = hashedPassword, Salt = salt, Programme = programme };

        _context.Users.Add(user);
        if(_context.SaveChanges() == 0) throw new ApiException($"The user could not be created - try again in a minute", 500);

        var claims = new Claim[] { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Role, "verification_token") };
        var verificationToken = _tokenService.GenerateToken(claims);

        _emailService.SendRegistrationVerificationEmail(user, verificationToken);

        return user;
    }

    private string EscapeName(string name)
    {
        return name.Trim(new Char[] { '<', '>', '{', '}' });
    }
}