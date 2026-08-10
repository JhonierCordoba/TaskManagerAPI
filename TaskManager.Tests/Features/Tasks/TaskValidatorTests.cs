using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Features.Tasks;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var command = new CreateTaskCommand("Write tests", null, TaskPriority.Medium, DateTime.UtcNow.AddDays(1), Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyTitle_IsInvalid()
    {
        var command = new CreateTaskCommand("", null, TaskPriority.Medium, null, Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithPastDueDate_IsInvalid()
    {
        var command = new CreateTaskCommand("Write tests", null, TaskPriority.Medium, DateTime.UtcNow.AddDays(-1), Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }
}

public class UpdateTaskStatusCommandValidatorTests
{
    private readonly UpdateTaskStatusCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var command = new UpdateTaskStatusCommand(Guid.NewGuid(), TaskManager.Domain.Enums.TaskStatus.InProgress, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyTaskId_IsInvalid()
    {
        var command = new UpdateTaskStatusCommand(Guid.Empty, TaskManager.Domain.Enums.TaskStatus.InProgress, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }
}
