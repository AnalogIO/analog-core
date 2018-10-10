using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Coffeecard.Models;
using System.Linq;

public class AccountService : IAccountService
{
    private readonly CoffeecardContext _context;
    private readonly ITokenService _tokenService;
    public AccountService(CoffeecardContext context, ITokenService tokenService) {
        _context = context;
        _tokenService = tokenService;
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
        if(user == null) throw new NotImplementedException();
        var hashedPw = HashBoi.Hash(password+user.Salt);
        if(user.Password.Equals(hashedPw)) {
            var claims = new Claim[] { new Claim(ClaimTypes.Email, username), new Claim(ClaimTypes.Name, user.Name) };
            var token =_tokenService.GenerateToken(claims);
            return token;
        }
        return null;
    }

    public User RegisterAccount(RegisterDTO registerDto)
    {
        throw new NotImplementedException();
    }
}