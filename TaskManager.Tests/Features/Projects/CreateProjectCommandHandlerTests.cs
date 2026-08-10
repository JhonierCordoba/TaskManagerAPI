using Moq;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Application.Features.Projects.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _handler = new CreateProjectCommandHandler(_projectRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_CreatesProjectForUser()
    {
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand("Portfolio site", "Personal project", userId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Portfolio site", result.Name);
        Assert.Equal("Personal project", result.Description);
        _projectRepository.Verify(r => r.AddAsync(It.Is<Project>(p => p.Name == "Portfolio site" && p.UserId == userId)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
