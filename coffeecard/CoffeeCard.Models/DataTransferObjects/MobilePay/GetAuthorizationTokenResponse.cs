using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay;

public class GetAuthorizationTokenResponse
{
    [Required]
    public required string AccessToken { get; set; }

    [Required]
    public required DateTimeOffset ExpiresOn { get; set; }

}