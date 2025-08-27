using MediatR;
using Microsoft.Extensions.Logging;
using BackendForFrontend.Application.Commands.Auth;
using BackendForFrontend.Application.Contracts.Auth;
using BackendForFrontend.Infrastructure.Interfaces;
using BackendForFrontend.Infrastructure.Models;

namespace BackendForFrontend.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IPaymentProviderClient _providerClient;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IPaymentProviderClient providerClient,
        ILogger<LoginCommandHandler> logger)
    {
        _providerClient = providerClient;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation("Processing login for user: {Username} with correlationId: {CorrelationId}",
            request.Username, correlationId);

        try
        {
            var providerRequest = new ProviderAuthRequest(request.Username, request.Password);
            var providerResponse = await _providerClient.AuthenticateAsync(providerRequest, correlationId, cancellationToken);

            var response = new LoginResponse(
                IsSuccess: providerResponse.Success,
                Jwt: providerResponse.Token,
                ExpiresAt: providerResponse.Expires
            );

            _logger.LogInformation("Login processed successfully for user: {Username} with correlationId: {CorrelationId}",
                request.Username, correlationId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user: {Username} with correlationId: {CorrelationId}",
                request.Username, correlationId);

            return new LoginResponse(false, null, null);
        }
    }
}

