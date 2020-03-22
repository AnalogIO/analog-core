namespace CoffeeCard.WebApi.Models.DataTransferObjects.Purchase
{
    public class IssueProductDto
    {
        public string IssuedBy { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}