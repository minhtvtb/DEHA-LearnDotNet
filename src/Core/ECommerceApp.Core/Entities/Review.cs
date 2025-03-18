using System;

namespace ECommerceApp.Core.Entities
{
    public class Review : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
} 