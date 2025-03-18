using FluentValidation;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Validators
{
    public class ProductValidator : AbstractValidator<ProductDto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(p => p.Description)
                .MaximumLength(2000).WithMessage("Product description cannot exceed 2000 characters");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("Category is required");
        }
    }
} 