using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    Domain.Enums.TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt
);