namespace NairaWallet.Application.Features.Wallets.Commands.ReverseTransfer;

public class ReverseTransferCommandValidator : AbstractValidator<ReverseTransferCommand>
{
    public ReverseTransferCommandValidator()
    {
        RuleFor(v => v.TransactionReference).NotEmpty()
            .Must(TransactionReference.IsValid).WithMessage("Invalid transaction reference.");
        RuleFor(v => v.Reason).MaximumLength(500);
    }
}