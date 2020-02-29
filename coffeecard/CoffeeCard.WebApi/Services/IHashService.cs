namespace CoffeeCard.WebApi.Services
{
    public interface IHashService
    {
        string GenerateSalt();
        string Hash(string password);
    }
}