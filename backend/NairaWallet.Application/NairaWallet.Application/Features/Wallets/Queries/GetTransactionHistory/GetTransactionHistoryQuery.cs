namespace NairaWallet.Application.Features.Wallets.Queries.GetTransactionHistory;

public class GetTransactionHistoryQuery : IRequest<PaginatedList<TransactionDto>>
{
    public string WalletTag { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public TransactionType? Type { get; init; }
    public TransactionStatus? Status { get; init; }
}
