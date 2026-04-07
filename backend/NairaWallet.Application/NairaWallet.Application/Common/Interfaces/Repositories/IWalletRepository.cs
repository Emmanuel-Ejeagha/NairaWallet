namespace NairaWallet.Application.Common.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByUserIsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByTagAsync(WalletTag tag, CancellationToken cancellationToken = default);
    void Add(Wallet wallet);
    void Update(Wallet wallet);
}
