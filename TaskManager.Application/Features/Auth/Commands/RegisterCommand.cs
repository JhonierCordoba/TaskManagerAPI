using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Auth.Commands;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<AuthResponseDto>;