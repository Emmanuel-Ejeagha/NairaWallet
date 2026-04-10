namespace NairaWallet.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, guid => UserId.FromGuid(guid));
        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, str => Email.Create(str))
            .HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber)
            .HasConversion(phone => phone == null ? null : phone.Value, str => PhoneNumber.CreateOptional(str)!);
        builder.Property(u => u.KYCStatus).HasConversion<string>();
        builder.Property(u => u.CreatedAtUtc);
        builder.Property(u => u.UpdatedAtUtc);
        builder.Property(u => u.RowVersion).IsRowVersion();

        builder.HasOne(u => u.Wallet)
            .WithOne()
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
