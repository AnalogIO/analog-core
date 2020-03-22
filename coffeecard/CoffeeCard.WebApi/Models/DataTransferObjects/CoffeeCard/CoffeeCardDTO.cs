namespace CoffeeCard.WebApi.Models.DataTransferObjects.CoffeeCard
{
    public class CoffeeCardDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int TicketsLeft { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}