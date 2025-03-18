using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Core.Entities;

namespace ECommerceApp.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderWithItemsAsync(int id);
        Task<IReadOnlyList<Order>> GetOrdersWithItemsAsync();
    }
} 