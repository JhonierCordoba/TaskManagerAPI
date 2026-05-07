using FluentValidation;

namespace TaskManager.Application.Features.Tasks.Commands;

public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}