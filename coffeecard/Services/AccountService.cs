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
        if (user == null) throw new NotImplementedException(); // use better exceptions that are forwarded back to the controller and returned to the caller
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
        throw new NotImplementedException();
    }
}