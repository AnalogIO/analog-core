namespace CoffeeCard.WebApi.Pages.Shared;

/// <summary>
/// All possible outcomes for user-facing webpages
/// </summary>
public enum Outcome
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Success,
    LinkExpiredOrUsed,
    UnhandledError,
    PinUpdateError,
    PasswordResetSuccess,
    EmailVerifiedSuccess,
    AccountDeletedSuccess,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

internal static class Outcomes
{
    public const string LinkExpiredOrUsed =
        "Looks like the link you used has expired or already been used. Request a new password in the app to verify your email.";
    public const string UnhandledError = "An unhandled error occurred. Try again later";
    public const string PinUpdateError =
        "An error occurred while updating your pin code. Please try again later or contact us at support@analogio.dk for further support";
    public const string PasswordResetSuccess = "Your password has now been reset";
    public const string EmailVerifiedSuccess = "Your email has been successfully verified";
    public const string AccountDeletedSuccess = "Your account has been successfully deleted";
    public const string UnknownError = "This is not the web page you are looking for";
}

internal static class OutcomeExtensions
{
    internal static string ToMessage(this Outcome outcome)
    {
        return outcome switch
        {
            Outcome.Success => string.Empty,
            Outcome.LinkExpiredOrUsed => Outcomes.LinkExpiredOrUsed,
            Outcome.UnhandledError => Outcomes.UnhandledError,
            Outcome.PinUpdateError => Outcomes.PinUpdateError,
            Outcome.PasswordResetSuccess => Outcomes.PasswordResetSuccess,
            Outcome.EmailVerifiedSuccess => Outcomes.EmailVerifiedSuccess,
            Outcome.AccountDeletedSuccess => Outcomes.AccountDeletedSuccess,
            _ => Outcomes.UnknownError,
        };
    }
}
