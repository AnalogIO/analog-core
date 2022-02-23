using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public class WebhookService : IWebhookService
    {
        private static readonly ICollection<string> DefaultEvents = new List<string> {"payment.reserved", "payment.expired"}.AsReadOnly(); 
        
        private readonly CoffeeCardContext _context;
        private readonly IMobilePayWebhooksService _mobilePayWebhooksService;
        private readonly MobilePaySettingsV2 _mobilePaySettings;

        private string _signatureKey;

        public WebhookService(CoffeeCardContext context, IMobilePayWebhooksService mobilePayWebhooksService, MobilePaySettingsV2 mobilePaySettings)
        {
            _context = context;
            _mobilePayWebhooksService = mobilePayWebhooksService;
            _mobilePaySettings = mobilePaySettings;
        }

        public async Task<string> SignatureKey()
        {
            if (_signatureKey == null)
            {
                await EnsureWebhookIsRegistered();
            }

            return _signatureKey;
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
                }
                
                webhook.Url = mobilePayWebhook.Url;
                webhook.SignatureKey = mobilePayWebhook.SignatureKey;
                webhook.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _signatureKey = mobilePayWebhook.SignatureKey;
            }
            catch (EntityNotFoundException)
            {
                await DisableAndRegisterNewWebhook(webhook);
            }
        }

        private async Task DisableAndRegisterNewWebhook(WebhookConfiguration webhook)
        {
            webhook.Status = WebhookStatus.Disabled;
            await _context.SaveChangesAsync();

            await RegisterWebhook();
        }

        private async Task RegisterWebhook()
        {
            var mobilePayWebhook = await _mobilePayWebhooksService.RegisterWebhook(_mobilePaySettings.WebhookUrl, DefaultEvents);

            var webhook = new WebhookConfiguration
            {
                Id = mobilePayWebhook.WebhookId,
                SignatureKey = mobilePayWebhook.SignatureKey,
                Status = WebhookStatus.Active,
                LastUpdated = DateTime.UtcNow
            };

            await _context.WebhookConfigurations.AddAsync(webhook);
            await _context.SaveChangesAsync();
            
            _signatureKey = mobilePayWebhook.SignatureKey;
        }
    }
}