using CoffeeCard.Common.Errors;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Enum for applications to log in to
    /// </summary>
    /// <example>
    /// Shifty
    /// </example>
    public enum LoginType
    {
        /// <summary>
        /// Log on Shifty website
        /// </summary>
        Shifty,

        /// <summary>
        /// Log on app
        /// </summary>
        App,
    }

    /// <summary>
    /// Extension methods for LoginType
    /// </summary>
    public static class LoginTypeExtensions
    {
        /// <summary>
        /// Get the deep link for the correct application
        /// </summary>
        /// <param name="loginType">The application to log in to</param>
        /// <param name="redirectUri">The redirect URI for the application</param>
        /// <param name="tokenHash">The generated token associated with a user</param>
        /// <returns>string</returns>
        /// <exception cref="ApiException">Unable to resolve application to log in to</exception>
        public static string GetDeepLink(
            this LoginType loginType,
            string redirectUri,
            string tokenHash
        )
        {
            return loginType switch
            {
                LoginType.Shifty => $"{redirectUri}auth?token={tokenHash}",
                LoginType.App => $"{redirectUri}login/auth/{tokenHash}",
                _ => throw new ApiException(
                    "Deep link for the given application has not been implemented"
                ),
            };
        }

        /// <summary>
        /// Get the deep link that lets a new user complete their registration.
        /// </summary>
        /// <param name="loginType">The application to register in</param>
        /// <param name="redirectUri">The redirect URI for the application</param>
        /// <param name="tokenHash">The generated registration token</param>
        /// <returns>string</returns>
        /// <exception cref="ApiException">Registration is not supported for the application</exception>
        public static string GetRegistrationDeepLink(
            this LoginType loginType,
            string redirectUri,
            string tokenHash
        )
        {
            return loginType switch
            {
                LoginType.App => $"{redirectUri}register/{tokenHash}",
                _ => throw new ApiException(
                    "Registration deep link for the given application has not been implemented"
                ),
            };
        }
    }
}
