namespace NairaWallet.Application.Common.Interfaces;

public interface IPaystackService
{
    Task<PaystackInitializeResponse> InitializeTransactionAsync(string email, decimal amount, string reference, CancellationToken cancellationToken = default);
    Task<PaystackVerifyResponse> VerifyTransactionAsync(string reference, CancellationToken cancellationToken = default);
    Task<PaystackWebhookEvent> ParseWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default);

    public record PaystackInitializeResponse(bool Status, string Message, string AuthorizationUrl, string AccessCode, string Reference);
    public record PaystackVerifyResponse(bool Status, string Message, decimal Amount, string Reference, string StatusCode);
    public record PaystackWebhookEvent(string Event, PaystackWebhookData Data);
    public record PaystackWebhookData(string Reference, decimal Amount, string Status);
}
