namespace Holocron.App.Api.Validators;

using FluentValidation;
using Holocron.App.Api.Models.Requests;

public class NewCommentRequestValidator : AbstractValidator<NewCommentRequest>
{
    public NewCommentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.DateWatched)
            .NotEmpty().WithMessage("DateWatched is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("DateWatched cannot be in the future.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10).WithMessage("Rating must be between 1 and 10.");

        RuleFor(x => x.Review)
            .NotEmpty().WithMessage("Review is required.")
            .MaximumLength(2000).WithMessage("Review cannot exceed 2000 characters.");
    }
}