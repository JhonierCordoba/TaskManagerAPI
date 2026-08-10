using Moq;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Application.Features.Projects.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Projects;

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _handler = new UpdateProjectCommandHandler(_projectRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ThrowsNotFoundException()
    {
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        var command = new UpdateProjectCommand(projectId, "New name", null, Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProject_ThrowsUnauthorizedException()
    {
        var project = Project.Create("Original name", null, Guid.NewGuid());
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UpdateProjectCommand(project.Id, "New name", null, Guid.NewGuid());

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOwnerUpdates_SavesChanges()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Original name", null, ownerId);
        _projectRepository.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UpdateProjectCommand(project.Id, "New name", "New description", ownerId);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("New name", project.Name);
        Assert.Equal("New description", project.Description);
        _projectRepository.Verify(r => r.UpdateAsync(project), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
