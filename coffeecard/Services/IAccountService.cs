using Coffeecard.Models;

public interface IAccountService
{
    User GetAccount(string token);
    User RegisterAccount(RegisterDTO registerDto);
}