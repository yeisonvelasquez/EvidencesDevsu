using Accounts.Application.Contracts;
using Accounts.Application.Exceptions;
using Accounts.Application.Mappings;
using Accounts.Application.Ports;
using Accounts.Domain;

namespace Accounts.Application;

/// <summary>Implementa las operaciones transaccionales de cuentas.</summary>
public sealed class AccountService(IAccountReadRepository readRepository, IAccountCommandRepository commandRepository) : IAccountService
{
    public async Task<IReadOnlyList<AccountResponse>> ListAccountsAsync(Guid? clientId, CancellationToken cancellationToken) =>
        (await readRepository.ListAsync(clientId, cancellationToken)).Select(x => x.ToResponse()).ToArray();

    public async Task<AccountResponse> GetAccountByNumberAsync(string accountNumber, CancellationToken cancellationToken) =>
        (await readRepository.GetByNumberAsync(accountNumber, cancellationToken) ?? throw new NotFoundException("Cuenta no encontrada.")).ToResponse();

    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        if (!await commandRepository.IsClientActiveAsync(request.ClientId, cancellationToken)) throw new ValidationException("El cliente no existe o está inactivo.");
        if (await commandRepository.ExistsByNumberAsync(request.AccountNumber, null, cancellationToken)) throw new ConflictException("El número de cuenta ya existe.");
        var account = new Account(request.AccountNumber, request.AccountType, request.InitialBalance, request.ClientId);
        await commandRepository.AddAsync(account, cancellationToken);
        await commandRepository.SaveChangesAsync(cancellationToken);
        return (await readRepository.GetByNumberAsync(account.AccountNumber, cancellationToken))!.ToResponse();
    }

    public async Task<AccountResponse> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await commandRepository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cuenta no encontrada.");
        account.SetDetails(request.AccountType, request.IsActive);
        await commandRepository.SaveChangesAsync(cancellationToken);
        return (await readRepository.GetByNumberAsync(account.AccountNumber, cancellationToken))!.ToResponse();
    }

}

