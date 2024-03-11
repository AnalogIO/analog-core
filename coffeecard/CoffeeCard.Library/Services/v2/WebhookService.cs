using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services.v2
{
    public class WebhookService : IWebhookService
    {
        private const string MpSignatureKeyCacheKey = "MpSignatureKey";

        private static readonly ISet<Events> DefaultEvents = new HashSet<Events>
            {Events.Payment_reserved, Events.Payment_expired, Events.Payment_cancelled_by_user};

        private readonly CoffeeCardContext _context;
        private readonly IMobilePayWebhooksService _mobilePayWebhooksService;
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly IMemoryCache _memoryCache;

        public WebhookService(CoffeeCardContext context, IMobilePayWebhooksService mobilePayWebhooksService,
            MobilePaySettingsV2 mobilePaySettings, IMemoryCache memoryCache)
        {
            _context = context;
            _mobilePayWebhooksService = mobilePayWebhooksService;
            _mobilePaySettings = mobilePaySettings;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get Mobile Pay Signature Key from Webhook configuration
        /// Signature Key is cached as value rarely changes and is looked up often. Cache expires every 24 hours or not used for more than 2 hours
        /// </summary>
        /// <returns>Signature Key</returns>
        public async Task<string> GetSignatureKey()
        {
            if (!_memoryCache.TryGetValue(MpSignatureKeyCacheKey, out string signatureKey))
            {
                signatureKey = await _context.WebhookConfigurations.Where(w => w.Status == WebhookStatus.Active)
                    .Select(w => w.SignatureKey).FirstAsync();

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddHours(24),
                    SlidingExpiration = TimeSpan.FromHours(2)
                };

                Log.Information("Set {SignatureKey} in Cache", MpSignatureKeyCacheKey);
                _ = _memoryCache.Set(MpSignatureKeyCacheKey, signatureKey, cacheExpiryOptions);
            }

            return signatureKey;
        }

        public async Task EnsureWebhookIsRegistered()
        {
            var webhooks = _context.WebhookConfigurations.Where(w => w.Status == WebhookStatus.Active);
            if (await webhooks.AnyAsync())
            {
                await SyncWebhook(await webhooks.FirstAsync());
                Log.Information("A MobilePay Webhook was already registered. Configuration has been synced");
            }
            else
            {
                await RegisterWebhook();
                Log.Information("A MobilePay Webhook has been registered");
            }
        }

        private async Task SyncWebhook(WebhookConfiguration webhook)
        {
            try
            {
                var mobilePayWebhook = await _mobilePayWebhooksService.GetWebhook(webhook.Id);

                if (!mobilePayWebhook.Url.Equals(_mobilePaySettings.WebhookUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await DisableAndRegisterNewWebhook(webhook);
                    return;
                }
            }
            catch (EntityNotFoundException)
            {
                await DisableAndRegisterNewWebhook(webhook);
            }
        }

        private async Task DisableAndRegisterNewWebhook(WebhookConfiguration webhook)
        {
            webhook.Status = WebhookStatus.Disabled;
            _ = await _context.SaveChangesAsync();

            await RegisterWebhook();
        }

        private async Task RegisterWebhook()
        {
            var mobilePayWebhook =
                await _mobilePayWebhooksService.RegisterWebhook(_mobilePaySettings.WebhookUrl, DefaultEvents);

            var webhook = new WebhookConfiguration
            {
                Id = mobilePayWebhook.WebhookId,
                Url = mobilePayWebhook.Url,
                SignatureKey = mobilePayWebhook.SignatureKey,
                Status = WebhookStatus.Active,
                LastUpdated = DateTime.UtcNow
            };

            _ = await _context.WebhookConfigurations.AddAsync(webhook);
            _ = await _context.SaveChangesAsync();
        }
    }
}