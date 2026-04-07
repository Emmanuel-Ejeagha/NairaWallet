namespace NairaWallet.Application.Common.Interfaces;

public interface IWalletTransferService
{
    Task<Transaction> TransferAsync(WalletId fromWalletId, WalletId toWalletId, Money amount, TransactionReference reference, string description, CancellationToken cancellationToken = default);
}
