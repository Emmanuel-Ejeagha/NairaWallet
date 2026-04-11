namespace NairaWallet.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Transaction entity.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Transaction?> GetByReferenceAsync(TransactionReference reference, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Reference == reference, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(WalletId walletId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public void Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
    }

    public void Update(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
    }
}