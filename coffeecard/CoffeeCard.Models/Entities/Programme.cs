using System.Collections.Generic;

namespace CoffeeCard.Models.Entities
{
    public class Programme
    {
        public int Id { get; set; }
        
        public string ShortName { get; set; }
        
        public string FullName { get; set; }
        
        public int SortPriority { get; set; }
        
        public virtual ICollection<User> Users { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ShortName)}: {ShortName}, {nameof(FullName)}: {FullName}, {nameof(SortPriority)}: {SortPriority}";
        }
    }
}