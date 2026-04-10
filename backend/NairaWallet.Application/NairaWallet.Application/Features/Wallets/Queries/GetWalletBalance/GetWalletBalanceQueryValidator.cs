namespace NairaWallet.Application.Features.Wallets.Queries.GetWalletBalance;

public class GetWalletBalanceQueryValidator : AbstractValidator<GetWalletBalanceQuery>
{
    public GetWalletBalanceQueryValidator()
    {
        RuleFor(x => x.WalletTag)
            .NotEmpty().Must(WalletTag.IsValid).WithMessage("Invalid wallet tag format.");
    }
}
