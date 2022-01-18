using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for initiating and retrieving purchases 
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/purchases")]
    public class PurchasesController : ControllerBase
    {
        /// <summary>
        /// Get all purchases
        /// </summary>
        /// <returns>List of purchases</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Purchase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Purchase>>> GetAllPurchases()
        {
            return new OkObjectResult(new List<Purchase>(2)
            {
                new Purchase
                {
                    Id = 122,
                    DateCreated = DateTime.Today,
                    ProductId = 1,
                    PurchaseStatus = PurchaseStatus.Completed
                },
                new Purchase
                {
                    Id = 132,
                    DateCreated = DateTime.Today,
                    ProductId = 2,
                    PurchaseStatus = PurchaseStatus.Completed
                }
            });
        }
        
        /// <summary>
        /// Get purchase
        /// </summary>
        /// <param name="purchaseId">Purchase Id</param>
        /// <returns>Purchase</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">No purchase found with purchase-id</response>
        [HttpGet("{purchase-id}")]
        [ProducesResponseType(typeof(Purchase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Purchase>> GetPurchase([FromRoute(Name = "purchase-id")] string purchaseId)
        {
            return new OkObjectResult(new Purchase
            {
                Id = 122,
                DateCreated = DateTime.Today,
                ProductId = 1,
                PurchaseStatus = PurchaseStatus.Completed
            });
        }

        /// <summary>
        /// Initiate a new payment. 
        /// </summary>
        /// <param name="initiateRequest"></param>
        /// <returns>Purchase with payment details</returns>
        /// <response code="201">Purchased initiated</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(typeof(InitiatePurchaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<InitiatePurchaseResponse>> InitiatePurchase([FromBody] InitatePurchaseRequest initiateRequest)
        {
            var purchaseResponse = new InitiatePurchaseResponse()
            {
                Id = 122,
                DateCreated = DateTime.Now,
                ProductId = 1,
                PurchaseStatus = PurchaseStatus.PendingPayment,
                PaymentDetails = new MobilePayPaymentDetails(
                    Guid.NewGuid().ToString(),
                    "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7",
                    "186d2b31-ff25-4414-9fd1-bfe9807fa8b7"
                )
            };

            return new CreatedAtActionResult(nameof(GetPurchase), nameof(PurchasesController), new {PurchaseId = purchaseResponse.Id},
                purchaseResponse);
        }
    }
}