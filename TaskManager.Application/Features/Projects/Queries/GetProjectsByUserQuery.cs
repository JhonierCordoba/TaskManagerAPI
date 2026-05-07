using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Projects.Queries;

public record GetProjectsByUserQuery(Guid UserId) : IRequest<IEnumerable<ProjectDto>>;