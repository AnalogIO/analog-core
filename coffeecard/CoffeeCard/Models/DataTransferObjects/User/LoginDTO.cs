using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.User
{
    public class LoginDTO
    {
        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }

        [Required] public string Version { get; set; }
    }
}