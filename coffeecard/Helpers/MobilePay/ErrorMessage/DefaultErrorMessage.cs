namespace coffeecard.Helpers.MobilePay.ErrorMessage
{
    public class DefaultErrorMessage : IMobilePayErrorMessage
    {
        // Introduce enum matching exact errorMessage
        public MobilePayErrorReason Reason { get; set; }

        public string GetErrorMessage()
        {
            return Reason.ToString();
        }

        public override string ToString()
        {
            return $"Reason = {Reason}";
        }
    }
}