using System.Net.Http.Headers;
using static NairaWallet.Application.Common.Interfaces.IPaystackService;

namespace NairaWallet.Infrastructure.Services;

/// <summary>
/// Paystack payment gateway integration using official SDK.
/// </summary>
public class PaystackService : IPaystackService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _secretKey;
    private readonly ILogger<PaystackService> _logger;

    public PaystackService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<PaystackService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _secretKey = configuration["Paystack:SecretKey"] ?? throw new InvalidOperationException("Paystack SecretKey missing");
        _logger = logger;
    }

    public async Task<PaystackInitializeResponse> InitializeTransactionAsync(string email, decimal amount, string reference, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        client.BaseAddress = new Uri("https://api.paystack.co/");

        var payload = new
        {
            email,
            amount = (int)(amount * 100), // Paystack uses kobo
            reference,
            callback_url = "https://yourdomain.com/payments/verify" // Should be configurable
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("transaction/initialize", content, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PaystackInitializeApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (!response.IsSuccessStatusCode || result == null || !result.Status)
        {
            _logger.LogError("Paystack init failed: {Message}", result?.Message);
            throw new Exception($"Paystack initialization failed: {result?.Message}");
        }

        return new PaystackInitializeResponse(
            Status: result.Status,
            Message: result.Message,
            AuthorizationUrl: result.Data.AuthorizationUrl,
            AccessCode: result.Data.AccessCode,
            Reference: result.Data.Reference
        );
    }

    public async Task<PaystackVerifyResponse> VerifyTransactionAsync(string reference, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        client.BaseAddress = new Uri("https://api.paystack.co/");

        var response = await client.GetAsync($"transaction/verify/{reference}", cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PaystackVerifyApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (!response.IsSuccessStatusCode || result == null || !result.Status)
        {
            _logger.LogError("Paystack verify failed: {Message}", result?.Message);
            throw new Exception($"Paystack verification failed: {result?.Message}");
        }

        return new PaystackVerifyResponse(
            Status: result.Status,
            Message: result.Message,
            Amount: result.Data.Amount / 100m,
            Reference: result.Data.Reference,
            StatusCode: result.Data.Status
        );
    }

    public async Task<PaystackWebhookEvent> ParseWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default)
    {
        // Verify signature using secret key
        var computedSignature = ComputeSignature(payload);
        if (computedSignature != signature)
            throw new UnauthorizedAccessException("Invalid webhook signature");

        var webhookEvent = JsonSerializer.Deserialize<PaystackWebhookApiEvent>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (webhookEvent == null)
            throw new InvalidOperationException("Invalid webhook payload");

        return new PaystackWebhookEvent(
            Event: webhookEvent.Event,
            Data: new PaystackWebhookData(webhookEvent.Data.Reference, webhookEvent.Data.Amount / 100m, webhookEvent.Data.Status)
        );
    }

    private string ComputeSignature(string payload)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private class PaystackInitializeApiResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackInitializeData Data { get; set; } = new();
    }

    private class PaystackInitializeData
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }

    private class PaystackVerifyApiResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackVerifyData Data { get; set; } = new();
    }

    private class PaystackVerifyData
    {
        public long Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private class PaystackWebhookApiEvent
    {
        public string Event { get; set; } = string.Empty;
        public PaystackWebhookApiData Data { get; set; } = new();
    }

    private class PaystackWebhookApiData
    {
        public string Reference { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}