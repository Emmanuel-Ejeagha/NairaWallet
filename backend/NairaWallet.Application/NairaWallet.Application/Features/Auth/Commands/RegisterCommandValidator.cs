namespace NairaWallet.Application.Features.Auth.Commands;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(v => v.PhoneNumber)
            .Matches(@"^(?:\+234|0)\d{10}$").WithMessage("Invalid phone number format.")
            .Must(phone => string.IsNullOrEmpty(phone) || PhoneNumber.IsValid(phone))
            .WithMessage("Invalid phone number format.");
    }
}
