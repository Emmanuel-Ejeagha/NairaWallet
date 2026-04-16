namespace NairaWallet.API.Endpoints;

public static class PaystackEndpoints
{
    public static void MapPaystackEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/paystack").WithTags("Paystack");

        group.MapPost("/initialize-funding", async (ISender sender, InitializeFundingCommand command, CancellationToken cancellationToken = default) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);

        })
        .WithName("InitializeFunding")
        .Produces<PaystackInitializeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Initializes a funding transaction with Paystack and returns the authorization URL and access code.")
        .RequireAuthorization();

        group.MapGet("/verify", async (ISender sender, string reference, CancellationToken cancellationToken = default) =>
        {
            var command = new VerifyFundingCommand { Reference = reference };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("VerifyFunding")
        .Produces<TransactionDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Verifies the status of a funding transaction using its reference and returns the transaction details.")
        .RequireAuthorization();

        group.MapPost("/webhook", async (ISender sender, HttpRequest request, CancellationToken cancellationToken = default) =>
        {
            var payload = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
            var signature = request.Headers["x-paystack-signature"].ToString();
            var command = new HandlePaystackWebhookCommand { Payload = payload, Signature = signature };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PaystackWebhook")
        .AllowAnonymous();
    }
}
