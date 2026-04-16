namespace NairaWallet.API.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wallet").WithTags("Wallet").RequireAuthorization();

        group.MapGet("/balance/{tag}", async (ISender sender, string tag, CancellationToken cancellationToken) =>
        {
            var query = new GetWalletBalanceQuery { WalletTag = tag };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetWalletBalance")
        .Produces<WalletDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .ProducesValidationProblem();

        group.MapGet("/transactions/{tag}", async (ISender sender, string tag, int page = 1, int pageSize = 10, DateTime? fromDate = null, DateTime? toDate = null, string? type = null, string? status = null, CancellationToken cancellationToken = default) =>
        {
            var query = new GetTransactionHistoryQuery
            {
                WalletTag = tag,
                PageNumber = page,
                PageSize = pageSize,
                FromDate = fromDate,
                ToDate = toDate,
                Type = Enum.TryParse<TransactionType>(type, true, out var parsedType) ? parsedType : null,
                Status = Enum.TryParse<TransactionStatus>(status, true, out var parsedStatus) ? parsedStatus : null
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetTransactionHistory")
        .Produces<PaginatedList<TransactionDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        group.MapPost("/tranfer", async (ISender sender, TransferCommand command, CancellationToken cancellationToken = default) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("TransferFunds")
        .Produces<TransactionDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Transfers funds from one wallet to another. Requires 'FromWalletTag', 'ToWalletTag', 'Amount', 'Description', and 'IdempotencyKey' in the request body.");

        group.MapPost("/reverse", async (ISender sender, ReverseTransferCommand command, CancellationToken cancellationToken = default) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ReverseTransaction")
        .Produces<TransactionDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithDescription("Reverses a transaction. Requires 'TransactionId' and 'IdempotencyKey' in the request body.");
    }
}
