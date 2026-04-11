using Microsoft.Extensions.Logging;

namespace NairaWallet.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with initial development data.
/// Uses domain methods (Credit/Debit) to ensure financial integrity.
/// </summary>
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger logger, IDateTimeProvider dateTimeProvider)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Database already contains users; skipping seed.");
            return;
        }

        logger.LogInformation("Seeding database with initial data...");

        // Helper to fund a wallet via system credit
        async Task FundWallet(Wallet wallet, Money amount, string description, ApplicationDbContext ctx)
        {
            var refNum = TransactionReference.Generate(dateTimeProvider);
            var creditTxn = wallet.Credit(amount, refNum, description);
            ctx.Transactions.Add(creditTxn);
            ctx.Wallets.Update(wallet);
        }

        // 1. Admin user
        var adminEmail = Email.Create("admin@nairawallet.com");
        var adminPasswordHash = passwordHasher.HashPassword("Admin@123");
        var adminUser = User.Create(adminEmail, adminPasswordHash, "System", "Administrator", null);
        adminUser.UpdateKYCStatus(KYCStatus.Verified);
        context.Users.Add(adminUser);

        var adminWalletTag = WalletTag.Create("@admin");
        var adminWallet = Wallet.Create(adminUser.Id, adminWalletTag);
        context.Wallets.Add(adminWallet);
        await FundWallet(adminWallet, Money.FromDecimal(100000m), "System seed funding for admin", context);

        // 2. Alice
        var aliceEmail = Email.Create("alice@example.com");
        var alicePasswordHash = passwordHasher.HashPassword("Alice@123");
        var aliceUser = User.Create(aliceEmail, alicePasswordHash, "Alice", "Johnson", PhoneNumber.CreateOptional("08012345678"));
        aliceUser.UpdateKYCStatus(KYCStatus.Verified);
        context.Users.Add(aliceUser);

        var aliceWalletTag = WalletTag.Create("@alice.johnson");
        var aliceWallet = Wallet.Create(aliceUser.Id, aliceWalletTag);
        context.Wallets.Add(aliceWallet);
        await FundWallet(aliceWallet, Money.FromDecimal(50000m), "System seed funding for Alice", context);

        // 3. Bob
        var bobEmail = Email.Create("bob@example.com");
        var bobPasswordHash = passwordHasher.HashPassword("Bob@123");
        var bobUser = User.Create(bobEmail, bobPasswordHash, "Bob", "Smith", PhoneNumber.CreateOptional("08098765432"));
        bobUser.UpdateKYCStatus(KYCStatus.Verified);
        context.Users.Add(bobUser);

        var bobWalletTag = WalletTag.Create("@bob.smith");
        var bobWallet = Wallet.Create(bobUser.Id, bobWalletTag);
        context.Wallets.Add(bobWallet);
        await FundWallet(bobWallet, Money.FromDecimal(25000m), "System seed funding for Bob", context);

        await context.SaveChangesAsync();

        // 4. Sample transfer from Alice to Bob (after balances are saved)
        var transferReference = TransactionReference.Generate(dateTimeProvider);
        var transferAmount = Money.FromDecimal(5000m);
        var aliceDebit = aliceWallet.Debit(transferAmount, transferReference, "Sample transfer to Bob");
        var bobCreditRef = TransactionReference.FromString($"CR-{transferReference.Value}");
        var bobCredit = bobWallet.Credit(transferAmount, bobCreditRef, "Sample transfer from Alice");

        context.Transactions.Add(aliceDebit);
        context.Transactions.Add(bobCredit);
        context.Wallets.Update(aliceWallet);
        context.Wallets.Update(bobWallet);

        await context.SaveChangesAsync();

        logger.LogInformation("Database seeding completed. Created admin, alice, bob, and one sample transfer.");
    }
}