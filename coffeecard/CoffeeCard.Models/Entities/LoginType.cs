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
        public static string getDeepLink(this LoginType loginType, string tokenHash)
        {
            // TODO: fix to correct URLs including tokenHash
            return loginType switch
            {
                LoginType.Shifty => $"https://shifty.prd.analogio.dk/token={tokenHash}",
                LoginType.App => "https://app.analogio.dk",
                _ => "https://shifty.coffee" // Register ??
            };
        }
    }
}