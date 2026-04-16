namespace NairaWallet.Application.Features.Wallets.Commands.ReverseTransfer;

public class ReverseTransferCommandHandler : IRequestHandler<ReverseTransferCommand, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReverseTransferCommandHandler> _logger;

    public ReverseTransferCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IDateTimeProvider dateTimeProvider,
        IApplicationDbContext context,
        ILogger<ReverseTransferCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(ReverseTransferCommand command, CancellationToken cancellationToken)
    {
        var reference = TransactionReference.FromString(command.TransactionReference);

        var originalTransaction = await _transactionRepository.GetByReferenceAsync(reference, cancellationToken);
        if (originalTransaction == null)
            throw new NotFoundException(nameof(Transaction), reference);

        if (originalTransaction.Type != TransactionType.Debit)
            throw new InvalidOperationException("Only debit transactions can be reversed.");

        var wallet = await _walletRepository.GetByIdAsync(originalTransaction.WalletId, cancellationToken);
        if (wallet == null)
            throw new NotFoundException(nameof(Wallet), originalTransaction.WalletId);

        var reversalTransaction = wallet.Reverse(originalTransaction, _dateTimeProvider);
        _walletRepository.Update(wallet);
        _transactionRepository.Add(reversalTransaction);
        _transactionRepository.Update(originalTransaction);

        await _context.SaveChangesAsync(cancellationToken);

        return new TransactionDto
        {
            Id = reversalTransaction.Id.ToString(),
            Reference = reversalTransaction.Reference.ToString(),
            Amount = reversalTransaction.Amount.Amount,
            Type = reversalTransaction.Type.ToString(),
            Status = reversalTransaction.Status.ToString(),
            Description = reversalTransaction.Description,
            CreatedAtUtc = reversalTransaction.CreatedAtUtc,
            ReversalTransactionId = reversalTransaction.ReversalTransactionId?.ToString(),
        };
    }
}
