namespace CoffeeCard.MobilePay.ErrorMessage
{
    public class InternalServerErrorMessage : IMobilePayErrorMessage
    {
        public string? CorrelationId { get; set; }
        public string? Errortype { get; set; }
        public string? Message { get; set; }

        public string GetErrorMessage()
        {
            return $"CorrelationId = {CorrelationId}, Errortype = {Errortype}, Message = {Message}";
        }

        public override string ToString()
        {
            return $"CorrelationId = {CorrelationId}, Errortype = {Errortype}, Message = {Message}";
        }
    }
}