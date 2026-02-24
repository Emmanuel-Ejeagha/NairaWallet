namespace NairaWallet.Domain.Entities;
/// <summary>
/// Immutable audit log entry for all state changes.
/// Financial invariants: Every financial operation must produce an audit log.
/// </summary>
public class AuditLog
{
    private AuditLog() { }

    public AuditLogId Id { get; private set; } = AuditLogId.New();
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Operation { get; private set; } = string.Empty;
    public string OldValues { get; private set; } = string.Empty;
    public string NewValues { get; private set; } = string.Empty;
    public string? UserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public AuditLog(string entityType, string entityId, string operation, string oldValues, string newValues, string? userId)
    {
        Id = AuditLogId.New();
        EntityType = entityType;
        EntityId = entityId;
        Operation = operation;
        OldValues = oldValues;
        NewValues = newValues;
        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
