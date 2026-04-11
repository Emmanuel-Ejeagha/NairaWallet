namespace NairaWallet.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Wallet entity.
/// </summary>
public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wallet?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<Wallet?> GetByTagAsync(WalletTag tag, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Tag == tag, cancellationToken);
    }

    public void Add(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
    }

    public void Update(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
    }
}