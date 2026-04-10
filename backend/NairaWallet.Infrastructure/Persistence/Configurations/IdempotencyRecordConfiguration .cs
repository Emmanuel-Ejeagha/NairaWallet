namespace NairaWallet.Infrastructure.Persistence.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, guid => IdempotencyRecordId.FromGuid(guid));
        builder.Property(i => i.Key).HasMaxLength(200);
        builder.HasIndex(i => i.Key).IsUnique();
        builder.Property(i => i.Response).HasColumnType("jsonb");
        builder.Property(i => i.CreateAtUtc);
        builder.Property(i => i.ExpiresAtUtc);
        builder.HasIndex(i => i.ExpiresAtUtc);
    }
}
