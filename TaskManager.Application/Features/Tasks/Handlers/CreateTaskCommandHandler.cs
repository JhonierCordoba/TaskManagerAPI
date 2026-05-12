using MediatR;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId)
            ?? throw new NotFoundException("Project", request.ProjectId);

        if (project.UserId != request.UserId)
            throw new UnauthorizedException();

        var task = TaskItem.Create(request.Title, request.Description, request.Priority, request.DueDate, request.ProjectId);

        await _taskRepository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        return new TaskDto(task.Id, task.Title, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt);
    }
}