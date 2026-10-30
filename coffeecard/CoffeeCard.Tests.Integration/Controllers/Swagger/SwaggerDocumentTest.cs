using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers.Swagger;

/// <summary>
/// Guards the generated OpenAPI contract against regressions that only show up
/// after clients are regenerated from the document.
/// </summary>
public sealed class SwaggerDocumentTest(CustomWebApplicationFactory<Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private async Task<JsonDocument> GetSpecAsync(string version)
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"/swagger/{version}/swagger.json");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    [Fact]
    public async Task V2_spec_keeps_operation_ids_and_receipt_polymorphism()
    {
        using var document = await GetSpecAsync("v2");
        var root = document.RootElement;
        var receipts = root.GetProperty("paths").GetProperty("/api/v2/receipts").GetProperty("get");

        Assert.Equal("Receipt_GetReceipts", receipts.GetProperty("operationId").GetString());

        var receiptSchema = root.GetProperty("components")
            .GetProperty("schemas")
            .GetProperty("ReceiptResponse");
        var items = receiptSchema
            .GetProperty("properties")
            .GetProperty("receipts")
            .GetProperty("items");

        var refs = items
            .GetProperty("oneOf")
            .EnumerateArray()
            .Select(item => item.GetProperty("$ref").GetString())
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains("#/components/schemas/PurchaseReceipt", refs);
        Assert.Contains("#/components/schemas/VoucherReceipt", refs);
        Assert.Contains("#/components/schemas/UsedTicketReceipt", refs);
        Assert.DoesNotContain("#/components/schemas/ReceiptBase", refs);

        var mapping = items.GetProperty("discriminator").GetProperty("mapping");
        Assert.True(mapping.TryGetProperty("Purchase", out _));
        Assert.True(mapping.TryGetProperty("Voucher", out _));
        Assert.True(mapping.TryGetProperty("UsedTicket", out _));
        Assert.False(mapping.TryGetProperty("ReceiptBase", out _));
    }

    [Fact]
    public async Task V2_spec_documents_security_per_operation()
    {
        using var document = await GetSpecAsync("v2");
        var root = document.RootElement;

        Assert.False(root.TryGetProperty("security", out _));

        var account = root.GetProperty("paths").GetProperty("/api/v2/account").GetProperty("get");
        Assert.True(HasScheme(account, "jwt"));
        Assert.False(HasScheme(account, "apikey"));

        var health = root.GetProperty("paths")
            .GetProperty("/api/v2/health/ping")
            .GetProperty("get");
        Assert.True(HasScheme(health, "apikey"));
        Assert.False(HasScheme(health, "jwt"));

        var login = root.GetProperty("paths")
            .GetProperty("/api/v2/account/login")
            .GetProperty("post");
        Assert.False(login.TryGetProperty("security", out _));
    }

    [Fact]
    public async Task V2_spec_marks_request_bodies_required_and_excludes_webhook()
    {
        using var document = await GetSpecAsync("v2");
        var root = document.RootElement;

        var login = root.GetProperty("paths")
            .GetProperty("/api/v2/account/login")
            .GetProperty("post");
        Assert.True(login.GetProperty("requestBody").GetProperty("required").GetBoolean());

        var purchase = root.GetProperty("paths")
            .GetProperty("/api/v2/purchases")
            .GetProperty("post");
        Assert.True(purchase.GetProperty("requestBody").GetProperty("required").GetBoolean());

        Assert.False(root.GetProperty("paths").TryGetProperty("/api/v2/mobilepay/webhook", out _));
    }

    [Fact]
    public async Task V2_spec_includes_schema_documentation_from_models()
    {
        using var document = await GetSpecAsync("v2");
        var name = document
            .RootElement.GetProperty("components")
            .GetProperty("schemas")
            .GetProperty("AddMenuItemRequest")
            .GetProperty("properties")
            .GetProperty("name");

        Assert.False(string.IsNullOrWhiteSpace(name.GetProperty("description").GetString()));
    }

    private static bool HasScheme(JsonElement operation, string scheme)
    {
        if (!operation.TryGetProperty("security", out var security))
            return false;

        return security
            .EnumerateArray()
            .Any(requirement => requirement.TryGetProperty(scheme, out _));
    }
}
