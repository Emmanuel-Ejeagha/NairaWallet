namespace NairaWallet.Domain.Entities;
/// <summary>
/// Tracks idempotency keys for external requests (Paystack webhooks, transfers).
/// Ensures that duplicate requests do not cause duplicate financial mutations.
/// </summary>
public class IdempotencyRecord
{
    private IdempotencyRecord() { }

    public IdempotencyRecordId Id { get; private set; } = IdempotencyRecordId.New();
    public string Key { get; private set; } = string.Empty;
    public string? Response { get; private set; }
    public DateTime CreateAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }

    public IdempotencyRecord(string key, TimeSpan ttl, string? response = null)
    {
        Id = IdempotencyRecordId.New();
        Key = key;
        Response = response;
        CreateAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = CreateAtUtc.Add(ttl);
    }

    public bool IsExpired(IDateTimeProvider dateTimeProvider) => dateTimeProvider.UtcNow > ExpiresAtUtc;
}
