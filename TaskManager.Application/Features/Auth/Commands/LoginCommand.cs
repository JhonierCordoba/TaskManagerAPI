using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;