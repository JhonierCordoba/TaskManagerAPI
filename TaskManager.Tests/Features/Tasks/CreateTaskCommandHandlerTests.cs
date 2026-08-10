using Moq;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Tasks;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepository = new();
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _handler = new CreateTaskCommandHandler(_taskRepository.Object, _projectRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ThrowsNotFoundException()
    {
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        var command = new CreateTaskCommand("Write tests", null, TaskPriority.Medium, null, projectId, Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProject_ThrowsUnauthorizedException()
    {
        var project = Project.Create("Portfolio site", null, Guid.NewGuid());
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskCommand("Write tests", null, TaskPriority.Medium, null, project.Id, Guid.NewGuid());

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOwnerCreatesTask_ReturnsPendingTask()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Portfolio site", null, ownerId);
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskCommand("Write tests", "Cover the handlers", TaskPriority.High, null, project.Id, ownerId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Write tests", result.Title);
        Assert.Equal(TaskManager.Domain.Enums.TaskStatus.Pending, result.Status);
        _taskRepository.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
