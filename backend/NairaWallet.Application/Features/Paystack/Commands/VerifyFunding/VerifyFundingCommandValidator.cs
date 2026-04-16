namespace NairaWallet.Application.Features.Paystack.Commands.VerifyFunding;

public class VerifyFundingCommandValidator : AbstractValidator<VerifyFundingCommand>
{
    public VerifyFundingCommandValidator()
    {
        RuleFor(x => x.Reference).NotEmpty().Must(TransactionReference.IsValid).WithMessage("Invalid transaction reference.");
    }
}
