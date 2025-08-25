using FluentValidation;
using Task_Management.Models;

namespace Task_Management.Validations
{
    public class ToDoListValidation : AbstractValidator<ToDoListModel>
    {
        public ToDoListValidation()
        {
            RuleFor(t => t.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(t => t.Description)
                .MaximumLength(700).WithMessage("Description must not exceed 700 characters");

            RuleFor(t => t.DueDate).NotEmpty().NotNull().Must(dueDate => dueDate > DateTime.Now)
				.WithMessage("Due Date must be a future date");

            RuleFor(t => t.Priority)
                .InclusiveBetween(0, 2).WithMessage("Priority must be between 0 and 2.");

            RuleFor(t => t.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required");

            RuleFor(t => t.UpdatedAt).NotEmpty().NotNull()
                .WithMessage("UpdatedAt cannot be earlier than CreatedAt");

            RuleFor(t => t.IsCompleted)
                .NotNull().WithMessage("IsCompleted must not be null");
        }
    }
}
