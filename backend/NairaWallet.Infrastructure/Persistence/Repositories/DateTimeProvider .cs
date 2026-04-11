namespace NairaWallet.Infrastructure.Persistence.Repositories;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
