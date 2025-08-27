using BackendForFrontend.Infrastructure.Models;

namespace BackendForFrontend.Infrastructure.Interfaces;

public interface IPaymentProviderClient
{
    Task<ProviderAuthResponse> AuthenticateAsync(
        ProviderAuthRequest request,
        string correlationId,
        CancellationToken cancellationToken = default);

    Task<ProviderPaymentResponse> ProcessPaymentAsync(
        ProviderPaymentRequest request,
        string? bearerToken,
        string correlationId,
        CancellationToken cancellationToken = default);
}

