namespace CoffeeCard.Library.Services
{
    public interface IHashService
    {
        string GenerateSalt();
        string Hash(string password);
    }
}
