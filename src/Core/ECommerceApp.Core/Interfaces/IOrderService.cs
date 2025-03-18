using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;

namespace ECommerceApp.Core.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task UpdateOrderStatusAsync(int id, OrderStatus status);
        Task DeleteOrderAsync(int id);
    }
} 