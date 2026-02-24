using System.Text.RegularExpressions;

namespace NairaWallet.Domain.ValueObjects;
/// <summary>
/// Unique, human-readable identifier for a wallet (e.g., @jogh.doe).
/// Immutable value object.
/// Format: ^@[a-zA-Z0-9._]{3,20}$
/// </summary>
public record WalletTag
{
    public string Value { get; }

    private WalletTag(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Invalid wallet tag format: {value}");
        Value = value;
    }

    public static WalletTag Create(string value) => new(value);

    public static WalletTag GenerateFromName(string firstName, string lastName)
    {
        var baseTag = $"@{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}";
        baseTag = baseTag.Length > 20 ? baseTag[..20] : baseTag;
        baseTag = Regex.Replace(baseTag, @"[^a-zA-Z0-9._]", "");
        if (!baseTag.StartsWith('@'))
            baseTag = "@" + baseTag;
        return new WalletTag(baseTag);
    }

    public static bool IsValid(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return false;
        return Regex.IsMatch(tag, @"^@[a-zA-Z0-9._]{3,20}$");
    }

    public override string ToString() => Value; 
}
