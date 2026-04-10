namespace NairaWallet.Application.Features.Paystack.Commands.InitializeFunding;

public class InitializeFundingCommandHandler : IRequestHandler<InitializeFundingCommand, PaystackInitializeResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPaystackService _paystackService;
    private readonly IDateTimeProvider _timeProvider;
    private readonly ILogger<InitializeFundingCommandHandler> _logger;

    public InitializeFundingCommandHandler(
        IWalletRepository walletRepository,
        IUserRepository userRepository,
        IPaystackService paystackService,
        IDateTimeProvider timeProvider,
        ILogger<InitializeFundingCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;
        _paystackService = paystackService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<PaystackInitializeResponse> Handle(InitializeFundingCommand command, CancellationToken cancellationToken)
    {
        var walletTag = WalletTag.Create(command.WalletTag);
        var wallet = await _walletRepository.GetByTagAsync(walletTag, cancellationToken);
        if (wallet == null)
            throw new NotFoundException(nameof(Wallet), walletTag);

        var user = await _userRepository.GetByIdAsync(wallet.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), wallet.UserId);

        var reference = TransactionReference.Generate(_timeProvider);

        var reponse = await _paystackService.InitializeTransactionAsync(
            user.Email.ToString(),
            command.Amount,
            reference.ToString(),
            cancellationToken);

        _logger.LogInformation("Initialized Paystack transaction for wallet {WalletTag} with reference {Reference}", walletTag, reference);

        return reponse;
    }
}
