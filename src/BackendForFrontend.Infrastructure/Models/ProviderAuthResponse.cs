using System.Text.Json.Serialization;

namespace BackendForFrontend.Infrastructure.Models;

public record ProviderAuthResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("expires")] DateTime? Expires
);

