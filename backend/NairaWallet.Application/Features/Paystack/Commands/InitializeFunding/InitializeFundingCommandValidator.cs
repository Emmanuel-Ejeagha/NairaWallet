namespace NairaWallet.Application.Features.Paystack.Commands.InitializeFunding;

public class InitializeFundingCommandValidator : AbstractValidator<InitializeFundingCommand>
{
    public InitializeFundingCommandValidator()
    {
        RuleFor(x => x.WalletTag)
            .NotEmpty().Must(WalletTag.IsValid).WithMessage("Wallet tag is invalid.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.CallbackUrl)
            .NotEmpty().WithMessage("Callback URL is required.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Callback URL must be a valid URL.");
    }
}