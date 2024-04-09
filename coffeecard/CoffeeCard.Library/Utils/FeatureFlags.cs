namespace CoffeeCard.Library.Utils
{
    public class FeatureFlags
    {
        /// <summary>
        /// Controls whether the API should manage the registration to the MobilePayWebhooks API at startup. Disabling this feature flag, assumes that the Webhook Registration is handled outside of the Analog Core API.
        /// </summary>
        public const string MobilePayManageWebhookRegistration = "MobilePayManageWebhookRegistration";
    }
}
