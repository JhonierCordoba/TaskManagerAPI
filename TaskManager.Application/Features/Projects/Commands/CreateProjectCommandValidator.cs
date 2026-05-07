using FluentValidation;

namespace TaskManager.Application.Features.Projects.Commands;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UserId).NotEmpty();
    }
}