using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using BackendForFrontend.Application.Commands.Auth;
using BackendForFrontend.Application.Contracts.Auth;

namespace BackendForFrontend.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/auth")]
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var response = await Mediator.Send(command);

        Response.Headers.Append("X-Correlation-ID", GetCorrelationId());

        return Ok(response);
    }
}
