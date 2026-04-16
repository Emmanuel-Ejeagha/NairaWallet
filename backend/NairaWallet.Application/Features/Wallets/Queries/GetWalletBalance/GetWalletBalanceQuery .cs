namespace NairaWallet.Application.Features.Wallets.Queries.GetWalletBalance;

public record GetWalletBalanceQuery : IRequest<WalletDto>
{
    public string WalletTag { get; init; } = string.Empty;
}
