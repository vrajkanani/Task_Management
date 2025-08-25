using FluentValidation;
using Task_Management.Models;

namespace Task_Management.Validations
{
    public class SubTaskValidation : AbstractValidator<SubTaskModel>
    {
        public SubTaskValidation()
        {
            RuleFor(subTask => subTask.MainTaskId)
                .NotEmpty().WithMessage("Main Task ID is required.")
                .GreaterThan(0).WithMessage("Main Task ID must be greater than 0.");

            RuleFor(subTask => subTask.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(subTask => subTask.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(700).WithMessage("Description cannot exceed 700 characters.");

            RuleFor(subTask => subTask.DueDate).NotEmpty().NotNull().Must(dueDate => dueDate > DateTime.Now)
				.WithMessage("Due Date must be a future date.");

            RuleFor(subTask => subTask.Priority)
                .InclusiveBetween(0, 2).WithMessage("Priority must be between 0 and 2.");

            RuleFor(subTask => subTask.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required.");

            RuleFor(subTask => subTask.UpdatedAt).NotEmpty().NotNull()
                .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");

            RuleFor(subTask => subTask.IsCompleted)
                .NotNull().WithMessage("IsCompleted must not be null.");
        }
    }
}
