namespace NairaWallet.Application.Features.Wallet.Queries.GetWalletBalance;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, WalletDto>
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletBalanceQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletDto> Handle(GetWalletBalanceQuery query, CancellationToken cancellationToken)
    {
        var tag = WalletTag.Create(query.WalletTag);
        var wallet = await _walletRepository.GetByTagAsync(tag, cancellationToken);

        if (wallet is null)
            throw new NotFoundException(nameof(Wallet), tag);

        return new WalletDto
        {
            Id = wallet.Id.ToString(),
            Tag = wallet.Tag.ToString(),
            Balance = wallet.Balance.Amount,
            Currency = wallet.Balance.Currency,
            CreatedAtUtc = wallet.CreatedAtUtc
        };
    }
}
