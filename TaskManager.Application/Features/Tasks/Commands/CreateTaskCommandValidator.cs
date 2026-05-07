using FluentValidation;

namespace TaskManager.Application.Features.Tasks.Commands;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue);
    }
}