namespace NairaWallet.Application.Features.Wallets.Commands.ReverseTransfer;

public record ReverseTransferCommand : IRequest<TransactionDto>
{
    public string TransactionReference { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}
