namespace NairaWallet.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
            throw new ValidationException(new[] { new ValidationFailure("Password", "Invalid email or password.") });

        var token = _tokenGenerator.GenerateToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        var userDto = new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber?.ToString(),
            KYCStatus = user.KYCStatus.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
        };

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            User = userDto
        };
    }
}
