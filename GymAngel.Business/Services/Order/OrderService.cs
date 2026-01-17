using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.OrderDTOs;
using GymAngel.Data.Repositories;
using GymAngel.Domain.Entities;

namespace GymAngel.Business.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly Auth.EmailService _emailService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Domain.Entities.User> _userManager;

        public OrderService(
            IOrderRepository orderRepository, 
            ICartRepository cartRepository,
            Auth.EmailService emailService,
            Microsoft.AspNetCore.Identity.UserManager<Domain.Entities.User> userManager)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO dto, int userId)
        {
            // 1. Get user's cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty. Cannot create order.");
            }

            // 2. Calculate amounts
            var subtotal = cart.CartItems.Sum(item => item.UnitPrice * item.Quantity);
            var discountAmount = 0m; // TODO: Apply discount code if provided
            var total = subtotal - discountAmount;

            // 3. Generate order number
            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

            // 4. Create order entity
            var order = new Domain.Entities.Order
            {
                OrderNumber = orderNumber,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                
                SubtotalAmount = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = total,
                
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                DeliveryAddress = dto.DeliveryAddress,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,
                Notes = dto.Notes,
                
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name, // Access through navigation property
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };

            // 5. Save order
            var createdOrder = await _orderRepository.CreateAsync(order);

            // 6. Send order confirmation email
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var emailSubject = $"Order Confirmation - {order.OrderNumber}";
                    var emailBody = Email.EmailTemplates.OrderConfirmation(createdOrder, user);
                    await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                }
            }
            catch (Exception ex)
            {
                // Log email error but don't fail the order
                Console.WriteLine($"Failed to send order confirmation email: {ex.Message}");
            }

            // 7. Clear cart after successful order
            await _cartRepository.ClearCartItemsAsync(cart.Id);

            // 8. Return response
            return MapToResponseDTO(createdOrder);
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            
            if (order == null)
                return null;
            
            // Check ownership
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to view this order.");
            
            return MapToResponseDTO(order);
        }

        public async Task<List<OrderSummaryDTO>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            
            return orders.Select(o => new OrderSummaryDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderItems?.Count ?? 0
            }).ToList();
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
                return false;
            
            // Check ownership
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to cancel this order.");
            
            // Only allow cancellation if order is pending
            if (order.Status != "Pending")
                throw new InvalidOperationException($"Cannot cancel order with status: {order.Status}");
            
            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);
            
            return true;
        }

        private OrderResponseDTO MapToResponseDTO(Domain.Entities.Order order)
        {
            return new OrderResponseDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                
                SubtotalAmount = order.SubtotalAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                DeliveryAddress = order.DeliveryAddress,
                City = order.City,
                District = order.District,
                Ward = order.Ward,
                Notes = order.Notes,
                
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                
                Items = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.UnitPrice * oi.Quantity
                }).ToList() ?? new List<OrderItemDTO>()
            };
        }
    }
}
