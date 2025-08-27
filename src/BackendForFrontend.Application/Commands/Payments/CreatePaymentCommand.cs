using MediatR;
using BackendForFrontend.Application.Contracts.Payments;

namespace BackendForFrontend.Application.Commands.Payments;

public record CreatePaymentCommand(
    decimal Amount,
    string Currency,
    string DestinationAccount,
    string? BearerToken
) : IRequest<PaymentResponse>;

