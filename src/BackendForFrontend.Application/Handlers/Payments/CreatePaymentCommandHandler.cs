using MediatR;
using Microsoft.Extensions.Logging;
using BackendForFrontend.Application.Commands.Payments;
using BackendForFrontend.Application.Contracts.Payments;
using BackendForFrontend.Infrastructure.Interfaces;
using BackendForFrontend.Infrastructure.Models;

namespace BackendForFrontend.Application.Handlers.Payments;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentResponse>
{
    private readonly IPaymentProviderClient _providerClient;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(
        IPaymentProviderClient providerClient,
        ILogger<CreatePaymentCommandHandler> logger)
    {
        _providerClient = providerClient;
        _logger = logger;
    }

    public async Task<PaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation("Processing payment of {Amount} {Currency} with correlationId: {CorrelationId}",
            request.Amount, request.Currency, correlationId);

        try
        {
            var providerRequest = new ProviderPaymentRequest(
                request.Amount,
                request.Currency,
                request.DestinationAccount);

            var providerResponse = await _providerClient.ProcessPaymentAsync(
                providerRequest,
                request.BearerToken,
                correlationId,
                cancellationToken);

            var response = new PaymentResponse(
                IsSuccess: providerResponse.Success,
                PaymentId: providerResponse.PaymentId,
                ProviderReference: providerResponse.Reference,
                ProcessedAt: providerResponse.Timestamp
            );

            _logger.LogInformation("Payment processed successfully with correlationId: {CorrelationId}", correlationId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment failed with correlationId: {CorrelationId}", correlationId);

            return new PaymentResponse(false, null, null, null);
        }
    }
}

