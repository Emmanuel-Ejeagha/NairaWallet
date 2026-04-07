namespace NairaWallet.Application.Common.Interfaces;

public interface ICurrentUserInterface
{
    UserId? UserId { get; }
    bool IsAuthenticated { get; }
}
