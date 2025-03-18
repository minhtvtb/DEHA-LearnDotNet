using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Infrastructure.Services;
using Moq;
using Xunit;

namespace ECommerceApp.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockMapper = new Mock<IMapper>();
            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, UserId = "user1", OrderDate = DateTime.Now, Status = OrderStatus.Pending },
                new Order { Id = 2, UserId = "user2", OrderDate = DateTime.Now, Status = OrderStatus.Processing }
            };

            var orderDtos = new List<OrderDto>
            {
                new OrderDto { Id = 1, UserId = "user1", OrderDate = DateTime.Now, Status = OrderStatus.Pending },
                new OrderDto { Id = 2, UserId = "user2", OrderDate = DateTime.Now, Status = OrderStatus.Processing }
            };

            _mockOrderRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<OrderDto>>(orders))
                .Returns(orderDtos);

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("user1", result.First().UserId);
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ShouldReturnUserOrders()
        {
            // Arrange
            string userId = "user1";
            var orders = new List<Order>
            {
                new Order { Id = 1, UserId = userId, OrderDate = DateTime.Now, Status = OrderStatus.Pending },
                new Order { Id = 3, UserId = userId, OrderDate = DateTime.Now, Status = OrderStatus.Shipped }
            };

            var orderDtos = new List<OrderDto>
            {
                new OrderDto { Id = 1, UserId = userId, OrderDate = DateTime.Now, Status = OrderStatus.Pending },
                new OrderDto { Id = 3, UserId = userId, OrderDate = DateTime.Now, Status = OrderStatus.Shipped }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<OrderDto>>(orders))
                .Returns(orderDtos);

            // Act
            var result = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(userId, item.UserId));
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            var orderDto = new OrderDto
            {
                Id = orderId,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItemDto>()
            };

            _mockOrderRepository.Setup(repo => repo.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync(order);

            _mockMapper.Setup(mapper => mapper.Map<OrderDto>(order))
                .Returns(orderDto);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.Equal("user1", result.UserId);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int orderId = 999;

            _mockOrderRepository.Setup(repo => repo.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateAndReturnOrder()
        {
            // Arrange
            var orderDto = new OrderDto
            {
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2, Price = 10.99m }
                }
            };

            var order = new Order
            {
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, Price = 10.99m }
                }
            };

            var createdOrder = new Order
            {
                Id = 1,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, Price = 10.99m }
                }
            };

            var createdOrderDto = new OrderDto
            {
                Id = 1,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, Price = 10.99m }
                }
            };

            _mockMapper.Setup(mapper => mapper.Map<Order>(orderDto))
                .Returns(order);

            _mockOrderRepository.Setup(repo => repo.AddAsync(order))
                .ReturnsAsync(createdOrder);

            _mockMapper.Setup(mapper => mapper.Map<OrderDto>(createdOrder))
                .Returns(createdOrderDto);

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("user1", result.UserId);
            Assert.Equal(OrderStatus.Pending, result.Status);
            Assert.Equal(1, result.OrderItems.Count());
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithValidIdAndStatus_ShouldUpdateStatus()
        {
            // Arrange
            int orderId = 1;
            OrderStatus newStatus = OrderStatus.Shipped;
            var order = new Order
            {
                Id = orderId,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Processing
            };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus);

            // Assert
            Assert.Equal(newStatus, order.Status);
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithInvalidId_ShouldNotCallUpdate()
        {
            // Arrange
            int orderId = 999;
            OrderStatus newStatus = OrderStatus.Shipped;

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus);

            // Assert
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CancelOrderAsync_WithValidPendingOrder_ShouldCancelOrder()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending
            };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Canceled, order.Status);
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_WithShippedOrder_ShouldReturnFalse()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                UserId = "user1",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Shipped
            };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.False(result);
            Assert.Equal(OrderStatus.Shipped, order.Status); // Status should not change
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CancelOrderAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            int orderId = 999;

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.False(result);
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }
    }
} 