namespace BackendForFrontend.Application.Contracts.Auth;

public record LoginRequestV2(
    string Email,
    string Password,
    string? MfaCode = null
);

