namespace NairaWallet.Domain.ValueObjects;

// User ID
public record UserId
{
    public Guid Value { get; }
    private UserId(Guid value) => Value = value;
    public static UserId New() => new(Guid.NewGuid());
    public static UserId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}

// Wallet ID
public record WalletId
{
    public Guid Value { get; }
    private WalletId(Guid value) => Value = value;
    public static WalletId New() => new(Guid.NewGuid());
    public static WalletId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}

// Transaction ID
public record TransactionId
{
    public Guid Value { get; }
    private TransactionId(Guid value) => Value = value;
    public static TransactionId New() => new(Guid.NewGuid());
    public static TransactionId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}

// AuditLog ID
public record AuditLogId
{
    public Guid Value { get; }
    private AuditLogId(Guid value) => Value = value;
    public static AuditLogId New() => new(Guid.NewGuid());
    public static AuditLogId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();

}

// IdempotencyRecord ID
public record IdempotencyRecordId
{
    public Guid Value { get; }
    private IdempotencyRecordId(Guid value) => Value = value;
    public static IdempotencyRecordId New() => new(Guid.NewGuid());
    public static IdempotencyRecordId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}

// FraudFlag ID 
public record FraudFlagId
{
    public Guid Value { get; }
    private FraudFlagId(Guid value) => Value = value;
    public static FraudFlagId New() => new(Guid.NewGuid());
    public static FraudFlagId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}




