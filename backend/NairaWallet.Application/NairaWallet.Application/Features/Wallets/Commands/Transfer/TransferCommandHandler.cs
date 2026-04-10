namespace NairaWallet.Application.Features.Wallets.Commands.Transfer;

public class TransferCommandHandler : IRequestHandler<TransferCommand, TransactionDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly IWalletTransferService _transferService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<TransferCommandHandler> _logger;

    public TransferCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IIdempotencyRepository idempotencyRepository,
        IWalletTransferService transferService,
        IDateTimeProvider dateTimeProvider,
        IApplicationDbContext context,
        ILogger<TransferCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _idempotencyRepository = idempotencyRepository;
        _transferService = transferService;
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(TransferCommand command, CancellationToken cancellationToken)
    {
        var idempotencyKey = $"transfer:{command.IdempotencyKey}";
        var existingRecord = await _idempotencyRepository.GetByKeyAsync(idempotencyKey, cancellationToken);
        if (existingRecord != null && !existingRecord.IsExpired(_dateTimeProvider))
            throw new InvalidOperationException("Duplicate request detected");

        var fromWalletTag = WalletTag.Create(command.FromWalletTag);
        var toWalletTag = WalletTag.Create(command.ToWalletTag);

        var fromWallet = await _walletRepository.GetByTagAsync(fromWalletTag, cancellationToken);
        var toWallet = await _walletRepository.GetByTagAsync(toWalletTag, cancellationToken);

        if (fromWallet == null)
            throw new NotFoundException(nameof(Wallet), fromWalletTag);
        if (toWallet == null)
            throw new NotFoundException(nameof(Wallet), toWalletTag);

        var reference = TransactionReference.Generate(_dateTimeProvider);

        var transaction = await _transferService.TransferAsync(
            fromWallet.Id,
            toWallet.Id,
            Money.FromDecimal(command.Amount),
            reference,
            command.Description,
            cancellationToken);

        var idempotencyRecord = new IdempotencyRecord(idempotencyKey, TimeSpan.FromHours(24), transaction.Id.ToString());
        _idempotencyRepository.Add(idempotencyRecord);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transfer completed: {FromWallet} -> {ToWallet}, Amount: {Amount}, Reference: {Reference}",
            fromWalletTag.Value, toWalletTag.Value, command.Amount, reference.Value);

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
