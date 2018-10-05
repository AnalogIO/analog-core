using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Coffeecard.Models;

public class AccountService : IAccountService
{
    public User GetAccount(string token)
    {
        throw new NotImplementedException();
    }

    public User RegisterAccount(RegisterDTO registerDto)
    {
        throw new NotImplementedException();
    }
}