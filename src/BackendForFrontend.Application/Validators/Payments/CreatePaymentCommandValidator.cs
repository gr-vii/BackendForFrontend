using FluentValidation;
using BackendForFrontend.Application.Commands.Payments;

namespace BackendForFrontend.Application.Validators.Payments;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000000);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3, 3)
            .Matches("^[A-Z]{3}$");

        RuleFor(x => x.DestinationAccount)
            .NotEmpty()
            .Length(5, 50)
            .Matches("^[A-Z0-9]+$");
    }
}

