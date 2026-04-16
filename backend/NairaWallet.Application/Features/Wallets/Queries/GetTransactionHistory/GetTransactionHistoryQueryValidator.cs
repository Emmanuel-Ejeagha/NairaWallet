namespace NairaWallet.Application.Features.Wallets.Queries.GetTransactionHistory;

public class GetTransactionHistoryQueryValidator : AbstractValidator<GetTransactionHistoryQuery>
{
    public GetTransactionHistoryQueryValidator()
    {
        RuleFor(v => v.WalletTag).NotEmpty()
            .Must(WalletTag.IsValid).WithMessage("Invalid wallet tag format.");
        RuleFor(v => v.PageNumber).GreaterThan(0);
        RuleFor(v => v.PageSize).InclusiveBetween(1, 100);
    }
}
