using System;

namespace NairaWallet.Domain.Exceptions;

public sealed class InsufficientFundsException : Exception
{
    public WalletId WalletId { get; }
    public Money Balance { get; }
    public Money AttemptedDebit { get; }

    public InsufficientFundsException(WalletId walletId, Money balance, Money attemptedDebit)
        : base($"Wallet {walletId} has insufficient funds. Balance: {balance}, attempted debit: {attemptedDebit}")
    {
        WalletId = walletId;
        Balance = balance;
        AttemptedDebit = attemptedDebit;
    }
}
