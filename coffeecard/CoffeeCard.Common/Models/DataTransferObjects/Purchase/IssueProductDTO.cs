namespace CoffeeCard.Common.Models.DataTransferObjects.Purchase
{
    public class IssueProductDto
    {
        public string IssuedBy { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}