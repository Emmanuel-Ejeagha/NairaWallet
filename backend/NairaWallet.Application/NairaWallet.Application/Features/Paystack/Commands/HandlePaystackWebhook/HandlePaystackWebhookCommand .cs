namespace NairaWallet.Application.Features.Paystack.Commands.HandlePaystackWebhook;

public class HandlePaystackWebhookCommand : IRequest<Unit>
{
    public string Payload { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}
