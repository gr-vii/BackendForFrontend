using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using BackendForFrontend.Application.Commands.Payments;
using BackendForFrontend.Application.Contracts.Payments;

namespace BackendForFrontend.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/payments")]
public class PaymentsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> CreatePayment(PaymentRequest request)
    {
        var bearerToken = HttpContext.Request.Headers.Authorization
            .FirstOrDefault()?.Replace("Bearer ", "");

        var command = new CreatePaymentCommand(
            request.Amount,
            request.Currency,
            request.DestinationAccount,
            bearerToken
        );

        var response = await Mediator.Send(command);

        Response.Headers.Append("X-Correlation-ID", GetCorrelationId());

        return Ok(response);
    }
}
