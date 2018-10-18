using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public class HashService : IHashService
    {
        private readonly RNGCryptoServiceProvider _rngCsp;
        private readonly SHA256Managed _hasher;
        public HashService()
        {
            _rngCsp = new RNGCryptoServiceProvider();
            _hasher = new SHA256Managed();
        }
        public string GenerateSalt()
        {
            var byteArr = new byte[256];
            _rngCsp.GetBytes(byteArr);
            string salt = BitConverter.ToString(byteArr);
            return salt;
        }

        public string Hash(string password)
        {
            byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = _hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
