using NairaWallet.Application.Common.Exceptions;

namespace NairaWallet.Infrastructure.Services;

/// <summary>
/// Domain service for executing wallet-to-wallet transfers with concurrency control and fraud checks.
/// </summary>
public class WalletTransferService : IWalletTransferService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly IFraudFlagRepository _fraudFlagRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<WalletTransferService> _logger;

    public WalletTransferService(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IIdempotencyRepository idempotencyRepository,
        IFraudFlagRepository fraudFlagRepository,
        IDateTimeProvider dateTimeProvider,
        IConnectionMultiplexer redis,
        ILogger<WalletTransferService> logger)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _idempotencyRepository = idempotencyRepository;
        _fraudFlagRepository = fraudFlagRepository;
        _dateTimeProvider = dateTimeProvider;
        _redis = redis;
        _logger = logger;
    }

    public async Task<Transaction> TransferAsync(WalletId fromWalletId, WalletId toWalletId, Money amount, TransactionReference reference, string description, CancellationToken cancellationToken)
    {
        var lockKey = $"lock:wallet:{fromWalletId}";
        var db = _redis.GetDatabase();
        var lockValue = Guid.NewGuid().ToString();
        var acquired = await db.StringSetAsync(lockKey, lockValue, TimeSpan.FromSeconds(30), When.NotExists);
        if (!acquired)
            throw new InvalidOperationException("Transfer is currently locked, please retry.");

        try
        {
            var velocityKey = $"velocity:transfer:{fromWalletId}:{_dateTimeProvider.UtcNow:yyyyMMddHHmm}";
            var count = await db.StringIncrementAsync(velocityKey);
            if (count == 1)
                await db.KeyExpireAsync(velocityKey, TimeSpan.FromMinutes(1));
            if (count > 5)
            {
                var fraudFlag = new FraudFlag("Wallet", fromWalletId.ToString(), "VelocityCheck", "Exceeded 5 transfers per minute");
                _fraudFlagRepository.Add(fraudFlag);
                throw new InvalidOperationException("Transfer limit exceeded. Maximum 5 transfers per minute.");
            }

          
            var fromWallet = await _walletRepository.GetByIdAsync(fromWalletId, cancellationToken);
            var toWallet = await _walletRepository.GetByIdAsync(toWalletId, cancellationToken);
            if (fromWallet == null || toWallet == null)
                throw new NotFoundException(nameof(Wallet), fromWallet == null ? fromWalletId : toWalletId);

            // Perform domain debit and credit
            var debitTransaction = fromWallet.Debit(amount, reference, description);
            var creditReference = TransactionReference.FromString($"CR-{reference.Value}");
            var creditTransaction = toWallet.Credit(amount, creditReference, description);

            _walletRepository.Update(fromWallet);
            _walletRepository.Update(toWallet);
            _transactionRepository.Add(debitTransaction);
            _transactionRepository.Add(creditTransaction);

            return debitTransaction; 
        }
        finally
        {
            var script = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";
            await db.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
        }
    }
}