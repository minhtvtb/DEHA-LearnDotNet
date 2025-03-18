using System;
using System.Collections.Generic;
using ECommerceApp.Core.Entities;

namespace ECommerceApp.Core.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
} 