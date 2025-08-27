using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendForFrontend.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected string GetCorrelationId()
    {
        return HttpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
               ?? HttpContext.TraceIdentifier;
    }
}

