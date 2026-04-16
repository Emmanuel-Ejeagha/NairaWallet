namespace NairaWallet.Application.Features.Paystack.Commands.InitializeFunding;

public record InitializeFundingCommand : IRequest<PaystackInitializeResponse>
{
    public string WalletTag { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string CallbackUrl { get; init; } = string.Empty;
}