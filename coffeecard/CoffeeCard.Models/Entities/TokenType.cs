using System;

namespace CoffeeCard.Models.Entities
{
    public enum TokenType
    {
        ResetPassword,
        DeleteAccount,
        MagicLink,
        Refresh
    }

    public static class TokenTypeExtensions
    {
        public static DateTime getExpiresAt(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ResetPassword => DateTime.UtcNow.AddDays(1),
                TokenType.DeleteAccount => DateTime.UtcNow.AddDays(1),
                TokenType.MagicLink => DateTime.UtcNow.AddMinutes(30),
                TokenType.Refresh => DateTime.UtcNow.AddMonths(1),
                _ => DateTime.UtcNow.AddDays(1)
            };
        }
    }
}