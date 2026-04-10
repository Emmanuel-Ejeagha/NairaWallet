namespace NairaWallet.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, guid => TransactionId.FromGuid(guid));
        builder.Property(t => t.WalletId)
            .HasConversion(id => id.Value, guid => WalletId.FromGuid(guid));
        builder.Property(t => t.Reference)
            .HasConversion(r => r.Value, str => TransactionReference.FromString(str))
            .HasMaxLength(50);
        builder.HasIndex(t => t.Reference).IsUnique();
        builder.Property(t  => t.Amount)
            .HasConversion(money => money.Amount, dec => Money.FromDecimal(dec))
            .HasPrecision(18, 2);
        builder.Property(t => t.Type).HasConversion<string>();
        builder.Property(t => t.Status).HasConversion<string>();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.CreatedAtUtc);
        builder.Property(t => t.UpdatedAtUtc);
        builder.Property(t => t.ReversedTransactionId)
            .HasConversion(id => id == null ? null : id.Value, guid => guid.HasValue ? TransactionId.FromGuid(guid.Value) : null);
        builder.Property(t => t.ReversalTransactionId)
            .HasConversion(id => id == null ? null : id.Value, guid => guid.HasValue ? TransactionId.FromGuid(guid.Value) : null);
        builder.Property(t => t.RowVersion).IsRowVersion()
    }
}
