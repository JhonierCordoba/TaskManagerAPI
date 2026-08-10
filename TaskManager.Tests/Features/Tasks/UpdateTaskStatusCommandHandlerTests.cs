using Moq;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Tasks;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepository = new();
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateTaskStatusCommandHandler _handler;

    public UpdateTaskStatusCommandHandlerTests()
    {
        _handler = new UpdateTaskStatusCommandHandler(_taskRepository.Object, _projectRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_ThrowsNotFoundException()
    {
        var taskId = Guid.NewGuid();
        _taskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((TaskItem?)null);

        var command = new UpdateTaskStatusCommand(taskId, TaskManager.Domain.Enums.TaskStatus.Completed, Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProject_ThrowsUnauthorizedException()
    {
        var project = Project.Create("Portfolio site", null, Guid.NewGuid());
        var task = TaskItem.Create("Write tests", null, TaskPriority.Medium, null, project.Id);
        _taskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UpdateTaskStatusCommand(task.Id, TaskManager.Domain.Enums.TaskStatus.Completed, Guid.NewGuid());

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOwnerUpdatesStatus_SavesChanges()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Portfolio site", null, ownerId);
        var task = TaskItem.Create("Write tests", null, TaskPriority.Medium, null, project.Id);
        _taskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UpdateTaskStatusCommand(task.Id, TaskManager.Domain.Enums.TaskStatus.Completed, ownerId);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(TaskManager.Domain.Enums.TaskStatus.Completed, task.Status);
        _taskRepository.Verify(r => r.UpdateAsync(task), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
