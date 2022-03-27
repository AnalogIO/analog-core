using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.User
{
    public class EmailInUseDTO
    {
        [Required]
        public bool InUse { get; set; }
    }
}