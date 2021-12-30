using System;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeCard.Library.Services
{
    public class HashService : IHashService
    {
        private readonly SHA256Managed _hasher;
        private readonly RNGCryptoServiceProvider _rngCsp;

        public HashService()
        {
            _rngCsp = new RNGCryptoServiceProvider();
            _hasher = new SHA256Managed();
        }

        public string GenerateSalt()
        {
            var byteArr = new byte[256];
            _rngCsp.GetBytes(byteArr);
            var salt = BitConverter.ToString(byteArr);
            return salt;
        }

        public string Hash(string password)
        {
            var byteArr = Encoding.UTF8.GetBytes(password);
            var hashBytes = _hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}