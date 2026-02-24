namespace NairaWallet.Domain.Entities;
/// <summary>
/// Represent a financial transaction on wallet.
/// Immutable once created. Reversal creates new transaction linking back.
/// </summary>
public class Transaction
{
    private Transaction() { }

    public TransactionId Id { get; private set; } = TransactionId.New();
    public WalletId WalletId { get; private set; }
    public TransactionReference Reference { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public TransactionId? ReversedTransactionId { get; private set; }
    public TransactionId? ReversalTransactionId { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Transaction(WalletId walletId, TransactionReference reference, Money amount, TransactionType type, string description)
    {
        Id = TransactionId.New();
        WalletId = walletId;
        Reference = reference;
        Amount = amount;
        Type = type;
        Description = description;
        Status = TransactionStatus.Success;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Transaction CreateCredit(WalletId walletId, Money amount, TransactionReference reference, string description)
    {
        return new Transaction(walletId, reference, amount, TransactionType.Credit, description);
    }

    public static Transaction CreateDebit(WalletId walletId, Money amount, TransactionReference reference, string description)
    {
        return new Transaction(walletId, reference, amount, TransactionType.Debit, description);
    }

    public static Transaction CreateReversal(WalletId walletId, Money amount, TransactionReference reference, TransactionId originalTransactionId)
    {
        var txn = new Transaction(walletId, reference, amount, TransactionType.Credit, $"Reversal of {originalTransactionId}");
        txn.ReversalTransactionId = originalTransactionId;
        return txn;
    }

    public void MarkAsReversed(TransactionId reversalTransactionId)
    {
        ReversalTransactionId = reversalTransactionId;
        Status = TransactionStatus.Reversed;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
