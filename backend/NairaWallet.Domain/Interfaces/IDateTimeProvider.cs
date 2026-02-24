namespace NairaWallet.Domain.Interfaces;
/// <summary>
/// Abstraction for time to enable testability and control over DateTime
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
