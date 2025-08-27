using Microsoft.Extensions.Diagnostics.HealthChecks;
using BackendForFrontend.Infrastructure.Interfaces;

namespace BackendForFrontend.WebApi.HealthChecks;

public class ProviderHealthCheck : IHealthCheck
{
    private readonly IPaymentProviderClient _providerClient;
    private readonly ILogger<ProviderHealthCheck> _logger;

    public ProviderHealthCheck(
        IPaymentProviderClient providerClient,
        ILogger<ProviderHealthCheck> logger)
    {
        _providerClient = providerClient;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Lightweight ping or health check to provider
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            var response = await httpClient.GetAsync("http://localhost:5001/health", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Provider is responding");
            }

            return HealthCheckResult.Degraded($"Provider returned status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider health check failed");
            return HealthCheckResult.Unhealthy("Provider is not responding", ex);
        }
    }
}

