namespace NairaWallet.Application.DTOs;

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string KYCStatus { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public WalletDto? Wallet { get; init; }
}
