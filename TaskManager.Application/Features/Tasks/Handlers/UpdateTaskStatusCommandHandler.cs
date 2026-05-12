using MediatR;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Handlers;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskStatusCommandHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId)
            ?? throw new NotFoundException("Task", request.TaskId);

        var project = await _projectRepository.GetByIdAsync(task.ProjectId)
            ?? throw new NotFoundException("Project", task.ProjectId);

        if (project.UserId != request.UserId)
            throw new UnauthorizedException();

        task.ChangeStatus(request.Status);

        await _taskRepository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
    }
}