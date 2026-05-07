using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands;

public record UpdateTaskStatusCommand(Guid TaskId, Domain.Enums.TaskStatus Status, Guid UserId) : IRequest;