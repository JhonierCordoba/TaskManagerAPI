using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Queries;

public record GetTasksByProjectQuery(
    Guid ProjectId,
    Guid UserId,
    Domain.Enums.TaskStatus? Status = null,
    TaskPriority? Priority = null
) : IRequest<IEnumerable<TaskDto>>;