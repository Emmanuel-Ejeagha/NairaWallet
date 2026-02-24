namespace NairaWallet.Domain.Entities;
/// <summary>
/// Represents a user of the NairaWallet system.
/// Financial invariants: A user can have exactly one active wallet.
/// Security: Email and Phonenumber must be unique
/// </summary>
public class User
{
    private User() { }

    public UserId Id { get; private set; } = UserId.New();
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public PhoneNumber? PhoneNumber { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public KYCStatus KYCStatus { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Wallet? Wallet { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private User(Email email, string passwordHash, string firstName, string lastName, PhoneNumber? phoneNumber)
    {
        Id = UserId.New();
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CreatedAtUtc = DateTime.UtcNow;
        KYCStatus = KYCStatus.Pending;
    }

    public static User Create(Email email, string passwordHash, string firstName, string lastName, PhoneNumber? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        return new User(email, passwordHash, firstName, lastName, phoneNumber);
    }

    public void UpdateKYCStatus(KYCStatus newStataus)
    {
        KYCStatus = newStataus;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, PhoneNumber? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
