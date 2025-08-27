using System.Text.Json.Serialization;

namespace BackendForFrontend.Infrastructure.Models;

public record ProviderPaymentRequest(
    [property: JsonPropertyName("total")] decimal Total,
    [property: JsonPropertyName("curr")] string Currency,
    [property: JsonPropertyName("dest")] string Destination
);

