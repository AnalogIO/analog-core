namespace coffeecard.Models.DataTransferObjects.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool PrivacyActivated { get; set; }
        public int ProgrammeId { get; set; }
        public int Level { get; set; }
        public int RequiredExp { get; set; }
        public int RankAllTime { get; set; }
        public int RankSemester { get; set; }
        public int RankMonth { get; set; }
    }
}
