using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Projects.Handlers;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(request.Name, request.Description, request.UserId);

        await _projectRepository.AddAsync(project);
        await _unitOfWork.CommitAsync();

        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt);
    }
}