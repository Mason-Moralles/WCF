using FluentValidation;

namespace TaskManager.Application.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("ProjectId must be positive.");

        RuleFor(x => x.AssigneeId)
            .GreaterThan(0).WithMessage("AssigneeId must be positive.");
    }
}

public record CreateTaskRequest(string Title, string Description, int Priority, int ProjectId, int AssigneeId, DateTime? DueDate);
