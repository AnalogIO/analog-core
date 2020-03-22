using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.User
{
    public class RegisterDto
    {
        [Required] public string Name { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }
    }
}