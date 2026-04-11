namespace NairaWallet.Infrastructure.Persistence.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly ApplicationDbContext _context;

    public IdempotencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.IdempotencyRecords.FirstOrDefaultAsync(i => i.Key == key, cancellationToken);
    }

    public void Add(IdempotencyRecord record)
    {
        _context.IdempotencyRecords.Add(record);
    }

    public async Task CleanupExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        await _context.IdempotencyRecords.Where(i => i.ExpiresAtUtc < now).ExecuteDeleteAsync(cancellationToken);
    }
}