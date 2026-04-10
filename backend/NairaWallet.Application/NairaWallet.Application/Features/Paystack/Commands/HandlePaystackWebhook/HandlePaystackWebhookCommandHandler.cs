namespace NairaWallet.Application.Features.Paystack.Commands.HandlePaystackWebhook;

public class HandlePaystackWebhookCommandHandler : IRequestHandler<HandlePaystackWebhookCommand, Unit>
{
    private readonly IPaystackService _paystackService;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<HandlePaystackWebhookCommandHandler> _logger;

    public HandlePaystackWebhookCommandHandler(
        IPaystackService paystackService,
        IIdempotencyRepository idempotencyRepository,
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IDateTimeProvider dateTimeProvider,
        IApplicationDbContext context,
        ILogger<HandlePaystackWebhookCommandHandler> logger)
    {
        _paystackService = paystackService;
        _idempotencyRepository = idempotencyRepository;
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(HandlePaystackWebhookCommand command, CancellationToken cancellationToken)
    {
        var webhookEvent = await _paystackService.ParseWebhookAsync(command.Payload, command.Signature, cancellationToken);

        var idempotencyKey = $"webhook:{webhookEvent.Data.Reference}";
        var existing = await _idempotencyRepository.GetByKeyAsync(idempotencyKey, cancellationToken);
        if (existing != null && !existing.IsExpired(_dateTimeProvider))
        {
            _logger.LogInformation("Duplicate webhook ignored: {Reference}", webhookEvent.Data.Reference);
            return Unit.Value;
        }

        if (webhookEvent.Event == "charge.success")
        {
            var reference = TransactionReference.FromString(webhookEvent.Data.Reference);
            var transaction = await _transactionRepository.GetByReferenceAsync(reference, cancellationToken);
            if (transaction != null)
            {
                _logger.LogInformation("Webhook received for unknown transaction reference: {Reference}", webhookEvent.Data.Reference);
            }
            else if (transaction!.Status != TransactionStatus.Success)
            {
                var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId, cancellationToken);
                if (wallet == null)
                    _logger.LogError("Wallet not found for transaction reference {TransactionId}", transaction.Id);
                else
                {
                    wallet.GetType().GetProperty("Balance")?.SetValue(wallet, wallet.Balance.Add(Money.FromDecimal(webhookEvent.Data.Amount)));
                    transaction.GetType().GetProperty("Status")?.SetValue(transaction, TransactionStatus.Success);
                    _walletRepository.Update(wallet);
                    _transactionRepository.Update(transaction);
                }
            }
        }

        var record = new IdempotencyRecord(idempotencyKey, TimeSpan.FromHours(24));
        _idempotencyRepository.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
