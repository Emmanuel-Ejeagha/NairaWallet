namespace NairaWallet.Domain.Entities;
/// <summary>
/// Represents a walllet owned by a user.
/// Financial invariants:
/// - Balance must never be negative
/// - All balance changes must be recorded via Transactions.
/// - Concurrency control via RowVersion.
/// </summary>
public class Wallet
{
    private readonly List<Transaction> _transaction = new();

    private Wallet() { }

    public WalletId Id { get; private set; } = WalletId.New();
    public UserId UserId { get; private set; }
    public WalletTag Tag { get; private set; }
    public Money Balance { get; private set; } = Money.Zero();
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<Transaction> Transactions => _transaction.AsReadOnly();

    private Wallet(UserId userId, WalletTag tag)
    {
        Id = WalletId.New();
        UserId = userId;
        Tag = tag;
        Balance = Money.Zero();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Wallet Create(UserId userId, WalletTag tag)
    {
        return new Wallet(userId, tag);
    }

    /// <summary>
    /// Credits the wallet with a positive amount.
    /// </summary>
    public Transaction Credit(Money amount, TransactionReference reference, string description)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Credit amount must be positive", nameof(amount));

        Balance = Balance.Add(amount);
        UpdatedAtUtc = DateTime.UtcNow;

        var transaction = Transaction.CreateCredit(Id, amount, reference, description);
        _transaction.Add(transaction);
        return transaction;
    }

    /// <summary>
    /// Debits the wallet if sufficient funds exist.
    /// </summary>
    public Transaction Debit(Money amount, TransactionReference reference, string description)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Debit must be positive", nameof(amount));

        if (Balance.Amount < amount.Amount)
            throw new InsufficientFundsException(Id, Balance, amount);

        Balance = Balance.Subtract(amount);
        UpdatedAtUtc = DateTime.UtcNow;

        var transaction = Transaction.CreateDebit(Id, amount, reference, description);
        _transaction.Add(transaction);
        return transaction;
    }

    /// <summary>
    /// Reverses a previous transaction (only with 30 minutes of original).
    /// </summary>
    public Transaction Reverse(Transaction originalTransaction, IDateTimeProvider dateTimeProvider)
    {
        if (originalTransaction.Type != TransactionType.Debit)
            throw new InvalidOperationException("Only debit transactions can be reversed.");

        if (originalTransaction.Status != TransactionStatus.Success)
            throw new InvalidOperationException("Only successful transactions can be reversed.");

        var now = dateTimeProvider.UtcNow;
        if (now > originalTransaction.CreatedAtUtc.AddMinutes(30))
            throw new InvalidOperationException("Revesal window of 30 minutes has expired.");

        if (_transaction.Any(t => t.ReversalTransactionId == originalTransaction.Id))
            throw new InvalidOperationException("Transaction already reversed.");

        var reversalReference = TransactionReference.FromString($"REVERSAL-{originalTransaction.Reference.Value}");
        var reversalTransaction = Transaction.CreateReversal(Id, originalTransaction.Amount, reversalReference, originalTransaction.Id);
        _transaction.Add(reversalTransaction);

        Balance = Balance.Add(originalTransaction.Amount);
        UpdatedAtUtc = now;

        originalTransaction.MarkAsReversed(reversalTransaction.Id);
        return reversalTransaction;
    }
}
