using MediatR;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Projects.Handlers;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Project", request.Id);

        if (project.UserId != request.UserId)
            throw new UnauthorizedException();

        project.Update(request.Name, request.Description);

        await _projectRepository.UpdateAsync(project);
        await _unitOfWork.CommitAsync();
    }
}