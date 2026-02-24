using System.Net.Mail;

namespace NairaWallet.Domain.ValueObjects;
/// <summary>
/// Email address value object with validation.
/// </summary>
public record Email
{
    public string Value { get; }

    private Email(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Invalid email format: {value}");
        value = value.Trim().ToLowerInvariant();
        Value = value;
    }

    public static Email Create(string value) => new(value);

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;
}
