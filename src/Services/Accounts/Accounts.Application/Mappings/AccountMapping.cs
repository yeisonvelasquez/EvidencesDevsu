using Accounts.Application.Contracts;
using Accounts.Application.ReadModels;
using Accounts.Domain;

namespace Accounts.Application.Mappings;

internal static class AccountMapping
{
    public static AccountResponse ToResponse(this AccountReadModel account)
    {
        return new AccountResponse(account.Id, account.AccountNumber, account.AccountType, account.Balance, account.IsActive, account.ClientId, account.ClientName);
    }

    public static TransactionResponse ToResponse(this Transaction transaction)
    {
        return new TransactionResponse(transaction.Id, transaction.AccountId, transaction.Type, transaction.Amount, transaction.PreviousBalance, transaction.ResultingBalance, transaction.OccurredAt);
    }
}
