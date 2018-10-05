using System.ComponentModel.DataAnnotations;

public class RegisterDTO {
    [Required]
    public string Name {get;set;}
    [Required]
    public string Email {get;set;}
    [Required]
    public string Password {get;set;}
    [Required]
    public int ProgrammeId {get;set;}
}