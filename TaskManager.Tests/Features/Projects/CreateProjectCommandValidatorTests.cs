using TaskManager.Application.Features.Projects.Commands;

namespace TaskManager.Tests.Features.Projects;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var result = _validator.Validate(new CreateProjectCommand("Portfolio site", "Description", Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_IsInvalid()
    {
        var result = _validator.Validate(new CreateProjectCommand("", "Description", Guid.NewGuid()));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyUserId_IsInvalid()
    {
        var result = _validator.Validate(new CreateProjectCommand("Portfolio site", "Description", Guid.Empty));

        Assert.False(result.IsValid);
    }
}
