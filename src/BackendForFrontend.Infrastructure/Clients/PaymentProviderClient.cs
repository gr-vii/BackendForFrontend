using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BackendForFrontend.Infrastructure.Interfaces;
using BackendForFrontend.Infrastructure.Models;

namespace BackendForFrontend.Infrastructure.Clients;

public class PaymentProviderClient : IPaymentProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentProviderClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PaymentProviderClient(HttpClient httpClient, ILogger<PaymentProviderClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ProviderAuthResponse> AuthenticateAsync(
        ProviderAuthRequest request,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling provider authentication with correlationId: {CorrelationId}", correlationId);

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Remove("X-Correlation-ID");
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

        var response = await _httpClient.PostAsync("/api/authenticate", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<ProviderAuthResponse>(responseJson, _jsonOptions);

        _logger.LogInformation("Provider authentication completed with correlationId: {CorrelationId}", correlationId);

        return result ?? throw new InvalidOperationException("Invalid provider response");
    }

    public async Task<ProviderPaymentResponse> ProcessPaymentAsync(
        ProviderPaymentRequest request,
        string? bearerToken,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling provider payment with correlationId: {CorrelationId}", correlationId);

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Remove("X-Correlation-ID");
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

        if (!string.IsNullOrEmpty(bearerToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _httpClient.PostAsync("/api/pay", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<ProviderPaymentResponse>(responseJson, _jsonOptions);

        _logger.LogInformation("Provider payment completed with correlationId: {CorrelationId}", correlationId);

        return result ?? throw new InvalidOperationException("Invalid provider response");
    }
}

