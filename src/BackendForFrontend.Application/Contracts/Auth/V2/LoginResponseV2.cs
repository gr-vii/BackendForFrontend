namespace BackendForFrontend.Application.Contracts.Auth;

public record LoginResponseV2(
    bool IsSuccess,
    string? Jwt,
    DateTime? ExpiresAt,
    bool MfaRequired
);

