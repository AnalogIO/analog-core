namespace CoffeeCard.Models.DataTransferObjects.User
{
    public class UpdateUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? PrivacyActivated { get; set; }
        public int? ProgrammeId { get; set; }
        public string Password { get; set; }
    }
}
