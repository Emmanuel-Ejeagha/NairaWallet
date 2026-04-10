namespace NairaWallet.Application.Features.Paystack.Commands.VerifyFunding;

public class VerifyFundingCommandHandler : IRequestHandler<VerifyFundingCommand, TransactionDto>
{
    private readonly IPaystackService _paystackService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<VerifyFundingCommandHandler> _logger;

    public VerifyFundingCommandHandler(
        IPaystackService paystackService,
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IIdempotencyRepository idempotencyRepository,
        IDateTimeProvider dateTimeProvider,
        IApplicationDbContext context,
        ILogger<VerifyFundingCommandHandler> logger)
    {
        _paystackService = paystackService;
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _idempotencyRepository = idempotencyRepository;
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(VerifyFundingCommand command, CancellationToken cancellationToken)
    {
        var idempotencyKey = $"paystack:verrify:{command.Reference}";
        var existing = await _idempotencyRepository.GetByKeyAsync(idempotencyKey, cancellationToken);
        if (existing != null && !existing.IsExpired(_dateTimeProvider))
        {
            _logger.LogInformation("Idempotent request. Returning cached response for reference {Reference}", command.Reference);
            var existingTransaction = await _transactionRepository.GetByReferenceAsync(TransactionReference.FromString(command.Reference), cancellationToken);
            if (existingTransaction != null)
                return MapToDto(existingTransaction);
        }

        var verification = await _paystackService.VerifyTransactionAsync(command.Reference, cancellationToken);
        if (!verification.Status || verification.StatusCode != "success")
            throw new InvalidOperationException($"Payment verification failed: {verification.Message}");

        var reference = TransactionReference.FromString(verification.Reference);
        var transaction = await _transactionRepository.GetByReferenceAsync(reference, cancellationToken);
        if (transaction == null)
            throw new InvalidOperationException($"No transaction found for reference {verification.Reference} ensure metadata was stored.");

        if (transaction.Status != TransactionStatus.Success)
        {
            var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId, cancellationToken);
            if (wallet == null)
                throw new NotFoundException(nameof(Wallet), transaction.WalletId);

            wallet.GetType().GetProperty("Balance")?.SetValue(wallet, wallet.Balance.Add(Money.FromDecimal(verification.Amount)));
            transaction.GetType().GetProperty("Status")?.SetValue(transaction, TransactionStatus.Success);
            _walletRepository.Update(wallet);
            _transactionRepository.Update(transaction);
        }

        var idempotencyRecord = new IdempotencyRecord(
            idempotencyKey,
            TimeSpan.FromHours(24),
            transaction.Id.ToString());

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment verification successful for reference {Reference}. Transaction ID: {TransactionId}", command.Reference, transaction.Id);

        return MapToDto(transaction);
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id.ToString(),
            Reference = transaction.Reference.ToString(),
            Amount = transaction.Amount.Amount,
            Type = transaction.Type.ToString(),
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }
}
