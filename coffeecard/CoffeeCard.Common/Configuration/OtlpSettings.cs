using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration;

public class OtlpSettings : IValidatable
{
    [Url]
    [Required]
    public string Endpoint { get; set; }
    public string Token { get; set; }

    [Required]
    public OtelProtocol Protocol { get; set; }

    public void Validate()
    {
        Validator.ValidateObject(this, new ValidationContext(this), true);
    }
}

public enum OtelProtocol
{
    Http,
    Grpc,
}
