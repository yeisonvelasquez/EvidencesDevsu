using Clients.Application.Contracts;
using FluentValidation;

namespace Clients.Application.Validators;

public sealed class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).Must(ClientValidationRules.IsSupportedGender).WithMessage("El género debe ser M o F.");
        RuleFor(x => x.Age).InclusiveBetween(0, 130);
        RuleFor(x => x.Identification).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator() => Include(new ClientFieldsValidator());
}

internal sealed class ClientFieldsValidator : AbstractValidator<UpdateClientRequest>
{
    public ClientFieldsValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).Must(ClientValidationRules.IsSupportedGender).WithMessage("El género debe ser M o F.");
        RuleFor(x => x.Age).InclusiveBetween(0, 130);
        RuleFor(x => x.Identification).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
    }
}

internal static class ClientValidationRules
{
    public static bool IsSupportedGender(string? value) => !string.IsNullOrWhiteSpace(value) && value.Trim().ToLowerInvariant() is "m" or "f" or "male" or "female" or "masculino" or "femenino";
}