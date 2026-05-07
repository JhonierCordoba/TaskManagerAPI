using MediatR;

namespace TaskManager.Application.Features.Projects.Commands;

public record UpdateProjectCommand(Guid Id, string Name, string? Description, Guid UserId) : IRequest;