using coffeecard.Models.DataTransferObjects.User;
using Coffeecard.Models;
using System.Collections.Generic;
using System.Security.Claims;

public interface IAccountService
{
    User GetAccountByClaims(IEnumerable<Claim> claims);
    User GetAccountByEmail(string email);
    User RegisterAccount(RegisterDTO registerDto);
    string Login(string username, string password, string version);
    bool VerifyRegistration(string token);
    User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDTO userDto);
    void UpdateExperience(int userId, int exp);
}