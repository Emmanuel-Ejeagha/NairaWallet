using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NairaWallet.Domain.ValueObjects;
/// <summary>
/// Globally unique transaction reference.
/// Format: NW-{UTC:yyyyMMddHHmmss}-{*_RANDMOM_UPPERCASE_ALPHANUMERIC}
/// Immutable value object.
/// </summary>
public record TransactionReference
{
    public string Value { get; }

    private TransactionReference(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Invalid transaction reference format: {value}");
        Value = value;
    }

    public static TransactionReference Generate(IDateTimeProvider dateTimeProvider)
    {
        var now = dateTimeProvider.UtcNow;
        var timeStamp = now.ToString("yyyyMMddHHmmss");
        var randomPart = GenerateRandomString(8);
        return new TransactionReference($"NW-{timeStamp}-{randomPart}");
    }

    public static TransactionReference FromString(string value) => new(value);

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var data = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }
        var result = new StringBuilder(length);
        foreach (byte b in data)
        {
            result.Append(chars[b % chars.Length]);
        }
        return result.ToString();
    }

    public static bool IsValid(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference)) return false;
        return Regex.IsMatch(reference, @"^NW-\d{14}-[A-A-Z0-9]{8}$");
    }

    public override string ToString() => Value;
}
