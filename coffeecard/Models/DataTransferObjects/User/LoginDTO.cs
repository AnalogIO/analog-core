using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Version { get; set; }

}