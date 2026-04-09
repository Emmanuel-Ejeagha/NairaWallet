namespace NairaWallet.Application.Features.Wallet.Queries.GetWalletBalance;

public record GetWalletBalanceQuery : IRequest<WalletDto>
{
    public string WalletTag { get; init; } = string.Empty;
}
