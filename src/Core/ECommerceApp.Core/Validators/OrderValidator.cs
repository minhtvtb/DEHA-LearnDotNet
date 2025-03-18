using FluentValidation;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Validators
{
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        public OrderValidator()
        {
            RuleFor(o => o.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(o => o.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required")
                .MaximumLength(500).WithMessage("Shipping address cannot exceed 500 characters");

            RuleFor(o => o.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required")
                .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters");

            RuleFor(o => o.OrderItems)
                .NotEmpty().WithMessage("Order must contain at least one item");
        }
    }
} 