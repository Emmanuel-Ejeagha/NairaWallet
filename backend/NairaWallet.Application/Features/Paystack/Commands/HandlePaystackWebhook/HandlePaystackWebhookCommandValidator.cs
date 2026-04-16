namespace NairaWallet.Application.Features.Paystack.Commands.HandlePaystackWebhook;

public class HandlePaystackWebhookCommandValidator : AbstractValidator<HandlePaystackWebhookCommand>
{
    public HandlePaystackWebhookCommandValidator()
    {
        RuleFor(x => x.Payload).NotEmpty().WithMessage("Payload is required.");
        RuleFor(x => x.Signature).NotEmpty().WithMessage("Signature is required.");
    }
}
