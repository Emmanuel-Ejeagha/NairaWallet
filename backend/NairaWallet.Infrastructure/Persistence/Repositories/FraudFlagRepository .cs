namespace NairaWallet.Infrastructure.Persistence.Repositories;

public class FraudFlagRepository : IFraudFlagRepository
{
    private readonly ApplicationDbContext _context;

    public FraudFlagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(FraudFlag fraudFlag)
    {
        _context.FraudFlags.Add(fraudFlag);
    }

    public async Task<IEnumerable<FraudFlag>> GetActiveFlagsByEntityIdAsync(string entityId, string entityType, CancellationToken cancellationToken = default)
    {
    
        return await _context.FraudFlags
            .Where(f => f.EntityId == entityId && f.EntityType == entityType)
            .ToListAsync(cancellationToken);
    }
}