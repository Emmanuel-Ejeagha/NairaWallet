using System;

namespace NairaWallet.Application.DTOs;

public record WalletDto
{
    public string Id { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = "NGN";
    public DateTime CreatedAtUtc { get; init; }
}
