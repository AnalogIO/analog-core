using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.User
{
    public class LoginDto
    {
        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }

        [Required] public string Version { get; set; }
    }
}