namespace BackendForFrontend.Application.Contracts.Auth;

public record LoginResponse(
    bool IsSuccess,
    string? Jwt,
    DateTime? ExpiresAt
);

