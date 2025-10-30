using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User;

/// <summary>
/// Resend Invite email request
/// </summary>
/// <example>
/// {
///     "email": "john@doe.com"
/// }
/// </example>
public class ResendAccountVerificationEmailRequest
{
    /// <summary>
    /// User email
    /// </summary>
    /// <value>Email</value>
    /// <example>john@doe.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
