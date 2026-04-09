namespace NairaWallet.Application.Features.Wallet.Commands.Transfer;

public record TransferCommand : IRequest<TransactionDto>
{
    public string FromWalletTag { get; init; } = string.Empty;
    public string ToWalletTag { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}
