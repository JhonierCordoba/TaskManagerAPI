using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Projects.Commands;

public record CreateProjectCommand(string Name, string? Description, Guid UserId) : IRequest<ProjectDto>;