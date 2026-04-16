namespace NairaWallet.Application.Features.Wallets.Queries.GetTransactionHistory;

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, PaginatedList<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;

    public GetTransactionHistoryQueryHandler(ITransactionRepository transactionRepository, IWalletRepository walletRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<PaginatedList<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var tag = WalletTag.Create(request.WalletTag);
        var wallet = await _walletRepository.GetByTagAsync(tag, cancellationToken);
        if (wallet == null)
            throw new NotFoundException(nameof(Wallet), tag);

        var transactions = await _transactionRepository.GetByWalletIdAsync(wallet.Id, request.PageNumber, request.PageSize, cancellationToken);
        var query = transactions.AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(t => t.CreatedAtUtc >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(t => t.CreatedAtUtc <= request.ToDate.Value);
        if (request.Type.HasValue)
            query = query.Where(t => t.Type == request.Type.Value);
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        var dtos = query.Select(t => new TransactionDto
        {
            Id = t.Id.ToString(),
            Reference = t.Reference.ToString(),
            Amount = t.Amount.Amount,
            Type = t.Type.ToString(),
            Status = t.Status.ToString(),
            Description = t.Description,
            CreatedAtUtc = t.CreatedAtUtc,
            ReversalTransactionId = t.ReversalTransactionId != null ? t.ReversalTransactionId.ToString() : null,
            ReversedTransactionId = t.ReversedTransactionId != null ? t.ReversedTransactionId.ToString() : null
        }).ToList();

        return new PaginatedList<TransactionDto>(dtos, dtos.Count, request.PageNumber, request.PageSize);
    }
}