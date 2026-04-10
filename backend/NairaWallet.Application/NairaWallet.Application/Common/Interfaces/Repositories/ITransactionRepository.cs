namespace NairaWallet.Application.Common.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByReferenceAsync(TransactionReference reference, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByWalletIdAsync(WalletId walletId, int page, int pagesize, CancellationToken cancellationToken = default);
    void Add(Transaction transaction);
    void Update(Transaction transaction);
}
