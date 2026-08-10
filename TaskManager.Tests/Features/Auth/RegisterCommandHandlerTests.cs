using Moq;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Application.Features.Auth.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_userRepository.Object, _unitOfWork.Object, _jwtService.Object);
    }

    [Fact]
    public async Task Handle_WithNewEmail_CreatesUserAndReturnsToken()
    {
        _userRepository.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((User?)null);
        _jwtService.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-jwt-token");

        var command = new RegisterCommand("Jane Doe", "new@test.com", "password123");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("fake-jwt-token", result.Token);
        Assert.Equal("Jane Doe", result.Name);
        Assert.Equal("new@test.com", result.Email);
        _userRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new@test.com")), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsInvalidOperationException()
    {
        var existingUser = User.Create("Existing", "taken@test.com", "hash");
        _userRepository.Setup(r => r.GetByEmailAsync("taken@test.com")).ReturnsAsync(existingUser);

        var command = new RegisterCommand("New Name", "taken@test.com", "password123");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
