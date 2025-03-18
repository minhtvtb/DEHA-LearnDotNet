using FluentValidation;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

            RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage("Category description cannot exceed 500 characters");
        }
    }
} 