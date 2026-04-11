using Microsoft.EntityFrameworkCore.Design;

namespace NairaWallet.Infrastructure.Persistence;

/// <summary>
/// Factory for creating ApplicationDbContext at design time (EF Core migrations).
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = "Host=localhost;Database=NairaWalletDb;Username=postgres;Password=postgres";
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}