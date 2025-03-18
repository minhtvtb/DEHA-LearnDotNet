using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;

namespace ECommerceApp.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            if (order == null) return null;
            
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            
            // Calculate total amount
            order.TotalAmount = 0;
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    item.UnitPrice = product.Price;
                    item.TotalPrice = item.UnitPrice * item.Quantity;
                    order.TotalAmount += item.TotalPrice;
                    
                    // Update product stock
                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }
            
            var result = await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(result);
        }

        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _orderRepository.UpdateAsync(order);
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            if (order != null)
            {
                // Restore product stock
                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }
                
                await _orderRepository.DeleteAsync(order);
            }
        }
    }
} 