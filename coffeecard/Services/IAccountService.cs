using Coffeecard.Models;

public interface IAccountService
{
    User GetAccount(string token);
    User RegisterAccount(RegisterDTO registerDto);

    User Login(string username, string password, string version);
}