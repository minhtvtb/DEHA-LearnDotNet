using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;

namespace ECommerceApp.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (User.IsInRole("Admin"))
            {
                // Admins can see all orders
                var allOrders = await _orderService.GetAllOrdersAsync();
                return View(allOrders);
            }
            else
            {
                // Regular users can only see their own orders
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return View(orders);
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Only allow users to view their own orders or admins to view any order
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var orderDto = new OrderDto
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                OrderDate = DateTime.UtcNow
            };
            
            return View(orderDto);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                // Ensure the order belongs to the current user
                orderDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orderDto.OrderDate = DateTime.UtcNow;
                orderDto.Status = OrderStatus.Pending;
                
                await _orderService.CreateOrderAsync(orderDto);
                return RedirectToAction(nameof(Index));
            }
            return View(orderDto);
        }

        // POST: Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.UpdateOrderStatusAsync(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Orders/Cancel/5
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Only allow users to cancel their own orders or admins to cancel any order
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Only allow cancellation of pending or processing orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                TempData["ErrorMessage"] = "Cannot cancel orders that have already been shipped or delivered.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(order);
        }

        // POST: Orders/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Only allow users to cancel their own orders or admins to cancel any order
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Only allow cancellation of pending or processing orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                TempData["ErrorMessage"] = "Cannot cancel orders that have already been shipped or delivered.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled);
            return RedirectToAction(nameof(Index));
        }
    }
} 