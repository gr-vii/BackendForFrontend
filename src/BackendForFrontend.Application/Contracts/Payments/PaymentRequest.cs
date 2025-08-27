namespace BackendForFrontend.Application.Contracts.Payments;

public record PaymentRequest(
    decimal Amount,
    string Currency,
    string DestinationAccount
);

