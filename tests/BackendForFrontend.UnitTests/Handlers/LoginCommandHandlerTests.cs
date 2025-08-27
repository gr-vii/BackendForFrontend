using Microsoft.Extensions.Logging;
using Moq;
using BackendForFrontend.Application.Commands.Auth;
using BackendForFrontend.Application.Handlers.Auth;
using BackendForFrontend.Infrastructure.Interfaces;
using BackendForFrontend.Infrastructure.Models;

namespace BackendForFrontend.UnitTests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IPaymentProviderClient> _mockProviderClient;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockProviderClient = new Mock<IPaymentProviderClient>();
        Mock<ILogger<LoginCommandHandler>> mockLogger = new();
        _handler = new LoginCommandHandler(_mockProviderClient.Object, mockLogger.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulAuthentication_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");
        var providerResponse = new ProviderAuthResponse(
            Success: true,
            Token: "jwt-token",
            Expires: DateTime.UtcNow.AddHours(1)
        );

        _mockProviderClient
            .Setup(x => x.AuthenticateAsync(
                It.IsAny<ProviderAuthRequest>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Jwt);
        Assert.NotNull(result.ExpiresAt);

        _mockProviderClient.Verify(x => x.AuthenticateAsync(
            It.Is<ProviderAuthRequest>(req => req.User == "test@example.com" && req.Password == "password123"),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProviderException_ReturnsFailureResponse()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        _mockProviderClient
            .Setup(x => x.AuthenticateAsync(
                It.IsAny<ProviderAuthRequest>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Provider unavailable"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Jwt);
        Assert.Null(result.ExpiresAt);
    }
}
