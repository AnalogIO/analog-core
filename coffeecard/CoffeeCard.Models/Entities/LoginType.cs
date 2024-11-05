namespace CoffeeCard.Models.Entities
{
    public enum LoginType
    {
        /// <summary>
        /// Log on Shifty website
        /// </summary>
        Shifty,
        /// <summary>
        /// Log on app
        /// </summary>
        App
    }

    public static class LoginTypeExtensions
    {
        public static string GetDeepLink(this LoginType loginType, string baseUrl, string tokenHash)
        {
            // TODO: fix to correct URLs including tokenHash
            return loginType switch
            {
                LoginType.Shifty => $"{baseUrl}auth?token={tokenHash}",
                LoginType.App => "https://app.analogio.dk",
                _ => "https://shifty.coffee" // Register ??
            };
        }
    }
}