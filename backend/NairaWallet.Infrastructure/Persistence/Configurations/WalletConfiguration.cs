namespace NairaWallet.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasConversion(id => id.Value, guid => WalletId.FromGuid(guid));
        builder.Property(w => w.UserId)
            .HasConversion(id => id.Value, guid => UserId.FromGuid(guid));
        builder.Property(w => w.Tag)
            .HasConversion(tag => tag.Value, str => WalletTag.Create(str))
            .HasMaxLength(25);
        builder.HasIndex(w => w.Tag).IsUnique();
        builder.Property(w => w.Balance)
            .HasConversion(money => money.Amount, dec => Money.FromDecimal(dec))
            .HasPrecision(18, 2);
        builder.Property(w => w.CreatedAtUtc);
        builder.Property(w => w.UpdatedAtUtc);
        builder.Property(w => w.RowVersion).IsRowVersion();

        builder.HasMany(w => w.Transactions)
            .WithOne()
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
