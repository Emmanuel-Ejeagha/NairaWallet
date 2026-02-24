using System.Text.RegularExpressions;

namespace NairaWallet.Domain.ValueObjects;
/// <summary>
/// Nigerian phone number value object.
/// Format: 08012345678, +2348012345678, etc
/// </summary>
public record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Invalid phone number format: {value}");
        Value = value;
    }

    public static PhoneNumber? CreateOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value);

    public static PhoneNumber Create(string value) => new(value);

    public static bool IsValid(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        // Nigerian numbers: 11 digits starting with 0, or 13 digits with +234
        return Regex.IsMatch(phoneNumber, @"^(0[789][01]\d{8})$|^(\+234[789][01]\d{8})$");
    }

    public override string ToString() => Value;
}
