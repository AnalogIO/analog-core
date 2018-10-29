using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeecard.Models.DataTransferObjects.Programme;

namespace coffeecard.Models.DataTransferObjects.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool PrivacyActivated { get; set; }
        public int ProgrammeId {get;set;}
    }
}
