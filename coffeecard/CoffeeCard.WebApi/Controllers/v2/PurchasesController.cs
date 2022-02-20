﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
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
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseController"/> class.
        /// </summary>
        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Get all purchases
        /// </summary>
        /// <returns>List of purchases</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SinglePurchaseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<SinglePurchaseResponse>>> GetAllPurchases()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Get purchase
        /// </summary>
        /// <param name="id">Purchase Id</param>
        /// <returns>Purchase</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">No purchase found with purchase-id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SinglePurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SinglePurchaseResponse>> GetPurchase([FromRoute] int id)
        {
            var purchase = await _purchaseService.GetPurchase(id);
            
            return new OkObjectResult(purchase);
        }

        /// <summary>
        /// Initiate a new payment. 
        /// </summary>
        /// <param name="initiateRequest">Initiate request</param>
        /// <returns>Purchase with payment details</returns>
        /// <response code="201">Purchased initiated</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(typeof(InitiatePurchaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<InitiatePurchaseResponse>> InitiatePurchase([FromBody] InitiatePurchaseRequest initiateRequest)
        {
            var purchaseResponse = await _purchaseService.InitiatePurchase(initiateRequest);

            return new CreatedAtActionResult(nameof(GetPurchase), nameof(PurchasesController), new {id = purchaseResponse.Id},
                purchaseResponse);
        }
    }
}