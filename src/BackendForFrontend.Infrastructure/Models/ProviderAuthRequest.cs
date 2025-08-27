using System.Text.Json.Serialization;

namespace BackendForFrontend.Infrastructure.Models;

public record ProviderAuthRequest(
    [property: JsonPropertyName("user")] string User,
    [property: JsonPropertyName("pwd")] string Password
);

