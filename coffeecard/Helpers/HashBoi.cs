using System;
using System.Security.Cryptography;

public class HashBoi {
    public static string GenerateSalt()
    {
        var rngCsp = new RNGCryptoServiceProvider();
        var byteArr = new byte[256];
        rngCsp.GetBytes(byteArr);
        string salt = BitConverter.ToString(byteArr);
        return salt;
    }

    public static string Hash(string password)
    {
        var hasher = new SHA256Managed();
        byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = hasher.ComputeHash(byteArr);
        return Convert.ToBase64String(hashBytes);
    }
}