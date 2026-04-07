namespace NairaWallet.Application.Common.Interfaces.Repositories;

public interface IFraudFlagRepository
{
    void Add(FraudFlag fraudFlag);
    Task<IEnumerable<FraudFlag>> GetActiveFlagsByEntityAsync(string entityId, string entityType, CancellationToken cancellationToken = default);
}
