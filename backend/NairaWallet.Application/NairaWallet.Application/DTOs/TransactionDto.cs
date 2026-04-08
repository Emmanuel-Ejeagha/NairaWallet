namespace NairaWallet.Application.DTOs;

public class TransactionDto
{
    public string Id { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Descripotion { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public string? ReversalTransactionId { get; init; }
    public string? ReversedTransactionId { get; init; }
}
