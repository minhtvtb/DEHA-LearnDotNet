using FluentValidation;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Validators
{
    public class ReviewValidator : AbstractValidator<ReviewDto>
    {
        public ReviewValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(r => r.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            RuleFor(r => r.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(r => r.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
        }
    }
} 