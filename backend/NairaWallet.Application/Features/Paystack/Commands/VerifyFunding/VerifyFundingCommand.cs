namespace NairaWallet.Application.Features.Paystack.Commands.VerifyFunding;

public record VerifyFundingCommand : IRequest<TransactionDto>
{
    public string Reference { get; init; } = string.Empty;
}
