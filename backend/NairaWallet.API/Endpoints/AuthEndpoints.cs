using NairaWallet.Application.Features.Auth.Commands.Login;
using NairaWallet.Application.Features.Auth.Commands.Register;

namespace NairaWallet.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async(ISender sender, RegisterCommand command, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);

        })
        .WithName("Register")
        .Produces<UserDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous()
        .WithDescription("Registers a new user and returns an authentication token.");

        group.MapPost("/login", async(ISender sender, LoginCommand command, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("Login")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous()
        .WithDescription("Authenticates a user and returns an authentication token.");
    }
}
