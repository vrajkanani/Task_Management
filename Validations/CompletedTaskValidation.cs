using FluentValidation;
using Task_Management.Models;

namespace Task_Management.Validations
{
    public class CompletedTaskValidation : AbstractValidator<CompletedTaskModel>
    {
        public CompletedTaskValidation()
        {
            RuleFor(task => task.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(task => task.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(task => task.DueDate).NotEmpty().NotNull()
                .WithMessage("Due Date must be a future date.");

            RuleFor(task => task.Priority)
                .InclusiveBetween(0, 2).WithMessage("Priority must be between 0 and 2.");

            RuleFor(task => task.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required.");

            RuleFor(task => task.UpdatedAt).NotEmpty().NotNull()
                .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");

            RuleFor(task => task.IsCompleted)
                .NotNull().WithMessage("IsCompleted must not be null.");
        }
    }
}
