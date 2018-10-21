using Coffeecard.Models;
using System.Collections.Generic;
using System.Security.Claims;

public interface IAccountService
{
    User GetAccountByClaims(IEnumerable<Claim> claims);
    User GetAccountByEmail(string email);
    User RegisterAccount(RegisterDTO registerDto);
    int GetIdFromEmail(string email);
    string Login(string username, string password, string version);
    bool VerifyRegistration(string token);
}