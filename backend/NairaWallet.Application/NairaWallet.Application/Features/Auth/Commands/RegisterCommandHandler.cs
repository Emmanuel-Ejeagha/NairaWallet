
using FluentValidation.Results;
using IDateTimeProvider = NairaWallet.Application.Common.Interfaces.IDateTimeProvider;
using ValidationException = NairaWallet.Application.Common.Exceptions.ValidationException;

namespace NairaWallet.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<UserDto> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
            throw new ValidationException(new[] { new ValidationFailure("Email", "Email already registered.") });

        var passwordHash = _passwordHasher.HashPassword(command.Password);
        var phoneNumber = PhoneNumber.CreateOptional(command.PhoneNumber);
        var user = User.Create(email, passwordHash, command.FirstName, command.LastName, phoneNumber);
        _userRepository.Add(user);

        var basetag = WalletTag.GenerateFromName(command.FirstName, command.LastName);

        var walletTag = await GenerateUniqueWalletTag(basetag, cancellationToken);
        var wallet = Wallet.Create(user.Id, walletTag);
        _walletRepository.Add(wallet);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User registered: {UserId}, wallet: {WalletTag}", user.Id, wallet.Tag);

        return new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber?.ToString(),
            KYCStatus = user.KYCStatus.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            Wallet = new WalletDto
            {
                Id = wallet.Id.ToString(),
                Tag = wallet.Tag.ToString(),
                Balance = wallet.Balance.Amount,
                CreatedAtUtc = wallet.CreatedAtUtc
            }
        };
    }

    private async Task<WalletTag> GenerateUniqueWalletTag(WalletTag baseTag, CancellationToken cancellationToken)
    {
        var tag = baseTag;
        var counter = 0;
        while (await _walletRepository.GetByTagAsync(tag, cancellationToken) != null)
        {
            counter++;
            tag = WalletTag.Create($"{baseTag.Value}{counter}");
            counter++;
            tag = WalletTag.Create($"{baseTag.Value}{counter}");
            if (counter > 100) 
                throw new Exception("Unable to generate unique wallet tag after 100 attempts.");
        }
        return tag;
    }
}
