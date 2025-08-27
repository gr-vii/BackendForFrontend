namespace BackendForFrontend.Application.Contracts.Payments;

public record PaymentResponse(
    bool IsSuccess,
    string? PaymentId,
    string? ProviderReference,
    DateTime? ProcessedAt
);

