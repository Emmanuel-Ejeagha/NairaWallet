namespace NairaWallet.Application.Common.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    void Add(IdempotencyRecord record);
    Task CleanupExpiredAsync(CancellationToken cancellationToken = default);
}
