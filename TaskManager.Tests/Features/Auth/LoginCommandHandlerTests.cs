using Moq;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Application.Features.Auth.Handlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_userRepository.Object, _jwtService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correct-password");
        var user = User.Create("Jane Doe", "jane@test.com", passwordHash);
        _userRepository.Setup(r => r.GetByEmailAsync("jane@test.com")).ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateToken(user)).Returns("fake-jwt-token");

        var command = new LoginCommand("jane@test.com", "correct-password");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("fake-jwt-token", result.Token);
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ThrowsUnauthorizedException()
    {
        _userRepository.Setup(r => r.GetByEmailAsync("ghost@test.com")).ReturnsAsync((User?)null);

        var command = new LoginCommand("ghost@test.com", "whatever");

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsUnauthorizedException()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correct-password");
        var user = User.Create("Jane Doe", "jane@test.com", passwordHash);
        _userRepository.Setup(r => r.GetByEmailAsync("jane@test.com")).ReturnsAsync(user);

        var command = new LoginCommand("jane@test.com", "wrong-password");

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
