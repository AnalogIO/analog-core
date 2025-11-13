using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

/// <summary>
/// Response object containing MobilePay API authorization token information.
/// </summary>
public class GetAuthorizationTokenResponse
{
    /// <summary>
    /// The OAuth access token used to authenticate requests to the MobilePay API.
    /// </summary>
    [Required]
    public required string AccessToken { get; set; }

    /// <summary>
    /// The date and time when the access token expires.
    /// </summary>
    [Required]
    public required DateTimeOffset ExpiresOn { get; set; }
}
