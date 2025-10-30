using System;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeCard.Library.Services
{
    public class HashService : IHashService
    {
        public string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var byteArr = new byte[256];
            rng.GetBytes(byteArr);
            var salt = BitConverter.ToString(byteArr);
            return salt;
        }

        public string Hash(string password)
        {
            var byteArr = Encoding.UTF8.GetBytes(password);
            using var hasher = SHA256.Create();
            var hashBytes = hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
