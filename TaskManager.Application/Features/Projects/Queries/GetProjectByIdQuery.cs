using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id, Guid UserId) : IRequest<ProjectDto>;