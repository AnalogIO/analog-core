using Coffeecard.Models;

public interface IAccountService
{
    User GetAccount(string token);
    User RegisterAccount(RegisterDTO registerDto);
    int GetIdFromEmail(string email);
    string Login(string username, string password, string version);
    bool VerifyRegistration(string token);
}