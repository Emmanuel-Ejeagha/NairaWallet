namespace NairaWallet.Domain.Entities;
/// <summary>
/// Rpresents a detected fraudulent activity or risk flag
/// </summary>
public class FraudFlag
{
    private FraudFlag() { }

    public FraudFlagId Id { get; private set; } = FraudFlagId.New();
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Rule { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    public FraudFlag(string entityType, string entityId, string rule, string description)
    {
        Id = FraudFlagId.New();
        EntityType = entityType;
        EntityId = entityId;
        Rule = rule;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
