using System;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeCard.Library.Services
{
    public class HashService : IHashService
    {
        public string GenerateSalt()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] byteArr = new byte[256];
            rng.GetBytes(byteArr);
            string salt = BitConverter.ToString(byteArr);
            return salt;
        }

        public string Hash(string password)
        {
            byte[] byteArr = Encoding.UTF8.GetBytes(password);
            using SHA256 hasher = SHA256.Create();
            byte[] hashBytes = hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}