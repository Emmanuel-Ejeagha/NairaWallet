namespace NairaWallet.Infrastructure.Persistence.Configurations;

public class FraudFlagConfiguration : IEntityTypeConfiguration<FraudFlag>
{
    public void Configure(EntityTypeBuilder<FraudFlag> builder)
    {
        builder.ToTable("FraudFlags");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, guid => FraudFlagId.FromGuid(guid));
        builder.Property(f => f.EntityType).HasMaxLength(100);
        builder.Property(f => f.EntityId).HasMaxLength(100);
        builder.Property(f => f.Rule).HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.CreatedAtUtc);
        builder.HasIndex(f => new { f.EntityType, f.EntityId });
    }
}