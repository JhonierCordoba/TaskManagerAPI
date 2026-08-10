using TaskManager.Application.Features.Auth.Commands;

namespace TaskManager.Tests.Features.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var result = _validator.Validate(new RegisterCommand("Jane Doe", "jane@test.com", "password123"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "jane@test.com", "password123")]
    [InlineData("Jane Doe", "not-an-email", "password123")]
    [InlineData("Jane Doe", "jane@test.com", "123")]
    public void Validate_WithInvalidCommand_IsInvalid(string name, string email, string password)
    {
        var result = _validator.Validate(new RegisterCommand(name, email, password));

        Assert.False(result.IsValid);
    }
}

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var result = _validator.Validate(new LoginCommand("jane@test.com", "password123"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "password123")]
    [InlineData("jane@test.com", "")]
    public void Validate_WithInvalidCommand_IsInvalid(string email, string password)
    {
        var result = _validator.Validate(new LoginCommand(email, password));

        Assert.False(result.IsValid);
    }
}
