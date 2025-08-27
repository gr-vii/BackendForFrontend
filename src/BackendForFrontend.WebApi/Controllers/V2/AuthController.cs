using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using BackendForFrontend.Application.Commands.Auth;
using BackendForFrontend.Application.Contracts.Auth;

namespace BackendForFrontend.WebApi.Controllers.V2;

[ApiVersion("2.0")]
[Route("v{version:apiVersion}/auth")]
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseV2>> Login(LoginRequestV2 request)
    {
        // V2 supports email-based login and MFA
        var command = new LoginCommand(request.Email, request.Password);
        var response = await Mediator.Send(command);

        Response.Headers.Append("X-Correlation-ID", GetCorrelationId());

        // Add deprecation warning for v1
        if (Request.Path.Value?.Contains("/v1/") == true)
        {
            Response.Headers.Append("Deprecation", "Sat, 31 Dec 2025 23:59:59 GMT");
            Response.Headers.Append("Sunset", "Sun, 30 Jun 2026 23:59:59 GMT");
            Response.Headers.Append("Link", "</v2/auth/login>; rel=\"successor-version\"");
        }

        return Ok(new LoginResponseV2(
            response.IsSuccess,
            response.Jwt,
            response.ExpiresAt,
            MfaRequired: false  // New field in V2
        ));
    }
}
