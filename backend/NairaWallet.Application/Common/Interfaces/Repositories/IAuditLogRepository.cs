namespace NairaWallet.Application.Common.Interfaces.Repositories;

public interface IAuditLogRepository
{
    void Add(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, string entityType, CancellationToken cancellationToken = default);
}
