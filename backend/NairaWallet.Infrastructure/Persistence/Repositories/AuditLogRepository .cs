namespace NairaWallet.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, string entityType, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityId == entityId && a.EntityType == entityType)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
