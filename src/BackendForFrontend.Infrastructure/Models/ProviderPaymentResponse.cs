using System.Text.Json.Serialization;

namespace BackendForFrontend.Infrastructure.Models;

public record ProviderPaymentResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("paymentId")] string? PaymentId,
    [property: JsonPropertyName("reference")] string? Reference,
    [property: JsonPropertyName("timestamp")] DateTime? Timestamp
);

