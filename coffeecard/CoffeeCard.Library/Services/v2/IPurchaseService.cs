using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IPurchaseService
    {
        /// <summary>
        /// Initiate a new purchase. Depending on the PaymentType, the purchase might be completed in the future
        /// </summary>
        /// <param name="initiateRequest">Initiate request with information on purchasable product and payment type</param>
        /// <param name="user">User</param>
        /// <returns>Response with Purchase details, status and payment details</returns>
        Task<InitiatePurchaseResponse> InitiatePurchase(InitiatePurchaseRequest initiateRequest, User user);

        /// <summary>
        /// Get purchase by Purchase Id
        /// </summary>
        /// <param name="purchaseId">Purchase Id</param>
        /// <param name="user">User</param>
        /// <returns>Purchase details</returns>
        Task<SinglePurchaseResponse> GetPurchase(int purchaseId, User user);
        
        /// <summary>
        /// Handle MobilePay webhook invocation and update purchase accordingly
        /// </summary>
        /// <param name="webhook">Webhook data object</param>
        /// <returns></returns>
        Task HandleMobilePayPaymentUpdate(MobilePayWebhook webhook);
    }
}