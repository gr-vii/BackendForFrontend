using BackendForFrontend.Application.Commands.Auth;
using BackendForFrontend.Application.Validators.Auth;

namespace BackendForFrontend.UnitTests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Theory]
    [InlineData("valid@example.com", "password123", true)]
    [InlineData("", "password123", false)]
    [InlineData("invalid-email", "password123", false)]
    [InlineData("valid@example.com", "", false)]
    [InlineData("valid@example.com", "short", false)]
    public void Validate_VariousInputs_ReturnsExpectedResult(string username, string password, bool expectedValid)
    {
        // Arrange
        var command = new LoginCommand(username, password);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }
}

