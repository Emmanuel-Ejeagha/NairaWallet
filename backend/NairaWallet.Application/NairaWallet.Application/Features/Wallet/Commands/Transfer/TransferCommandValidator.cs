namespace NairaWallet.Application.Features.Wallet.Commands.Transfer;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(v => v.FromWalletTag).NotEmpty().Must(WalletTag.IsValid)
            .WithMessage("Invalid source wallet tag.");
        RuleFor(v => v.ToWalletTag).NotEmpty().Must(WalletTag.IsValid)
            .WithMessage("Invalid destination wallet tag.");
        RuleFor(v => v.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(v => v.IdempotencyKey).NotEmpty().WithMessage("Idempotency key is required.");
    }
}
