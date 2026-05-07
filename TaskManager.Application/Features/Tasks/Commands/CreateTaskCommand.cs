using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands;

public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid ProjectId,
    Guid UserId
) : IRequest<TaskDto>;