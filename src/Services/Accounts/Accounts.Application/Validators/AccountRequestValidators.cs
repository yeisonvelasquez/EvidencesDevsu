using Accounts.Application.Contracts;
using FluentValidation;

namespace Accounts.Application.Validators;

public sealed class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.AccountType).NotEmpty().Must(BeSupportedAccountType).WithMessage("El tipo de cuenta debe ser Savings o Checking.");
        RuleFor(x => x.InitialBalance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ClientId).NotEmpty();
    }

    private static bool BeSupportedAccountType(string value) => value.Trim().ToLowerInvariant() is "savings" or "ahorros" or "checking" or "corriente";
}

public sealed class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.AccountType).NotEmpty().Must(value => value.Trim().ToLowerInvariant() is "savings" or "ahorros" or "checking" or "corriente").WithMessage("El tipo de cuenta debe ser Savings o Checking.");
    }
}

public sealed class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.IdempotencyKey).MaximumLength(100).When(x => x.IdempotencyKey is not null);
    }
}

public sealed class StatementRequestValidator : AbstractValidator<StatementRequest>
{
    public StatementRequestValidator()
    {
        RuleFor(x => x.Identificacion).NotEmpty().MaximumLength(40);
        RuleFor(x => x).Must(x => x.FechaInicio <= x.FechaFin).WithMessage("La fecha inicial no puede ser posterior a la fecha final.");
    }
}