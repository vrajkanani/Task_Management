using FluentValidation;
using Task_Management.Models;

namespace TODOAPI.Validations
{

    public class CompleteSubTaskValidator : AbstractValidator<CompleteSubTaskModel>
    {
        public CompleteSubTaskValidator() 
        {
            RuleFor(x => x.CompleteSubTaskId)
            .GreaterThan(0).WithMessage("CompleteSubTaskId must be greater than 0.");

            RuleFor(x => x.SubTaskId)
                .NotEmpty().WithMessage("SubTaskId is required.");

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(700).WithMessage("Description cannot exceed 700 characters.");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Now).When(x => x.DueDate.HasValue).WithMessage("DueDate must be greater than or equal to the current date.");

            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 2).WithMessage("Priority must be a valid between 0 to 2.");

            RuleFor(x => x.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required.");

            RuleFor(x => x.CompletedAt)
                .NotEmpty().WithMessage("CompletedAt is required.")
                .GreaterThanOrEqualTo(x => x.CreatedAt).WithMessage("CompletedAt must be greater than or equal to CreatedAt.");
        }
    }
}
