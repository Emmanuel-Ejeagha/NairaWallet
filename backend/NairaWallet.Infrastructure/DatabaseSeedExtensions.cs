using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NairaWallet.Infrastructure;

public static class DatabaseSeedExtensions
{
    /// <summary>
    /// Ensures the database is migrated and seeded with development data.
    /// Call this from WebApi Program.cs only in Development environment.
    /// </summary>
    public static async Task SeedDatabaseAsync(this IApplicationBuilder app, bool isDevelopment)
    {
        if (!isDevelopment)
            return;

        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ApplicationDbContextSeed");
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        await ApplicationDbContextSeed.SeedAsync(context, passwordHasher, logger, dateTimeProvider);
    }
}