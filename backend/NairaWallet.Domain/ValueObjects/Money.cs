namespace NairaWallet.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; } = "NGN";

    private Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative", nameof(amount));
        Amount = amount;
    }

    public static Money FromDecimal(decimal amount) => new(amount);
    public static Money Zero() => new(0);

    public Money Add(Money other)
    {
        if (other.Currency != Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount + other.Amount);
    }

    public Money Subtract(Money other)
    {
        if (other.Currency != Currency)
            throw new InvalidOperationException("Currency mismatch");

        return new Money(Amount - other.Amount);
    }

    public override string ToString() => $"{Currency} {Amount:F2}";
}
