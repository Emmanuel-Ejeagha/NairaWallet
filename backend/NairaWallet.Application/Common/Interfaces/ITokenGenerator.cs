namespace NairaWallet.Application.Common.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
