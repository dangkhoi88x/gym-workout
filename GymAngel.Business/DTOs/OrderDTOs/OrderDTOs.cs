using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymAngel.Business.DTOs.OrderDTOs
{
    // Request DTO for creating a new order
    public class CreateOrderDTO
    {
        [Required(ErrorMessage = "Receiver name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string ReceiverName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Invalid Vietnamese phone number")]
        public string ReceiverPhone { get; set; } = null!;

        [Required(ErrorMessage = "Delivery address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string DeliveryAddress { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "District is required")]
        public string District { get; set; } = null!;

        [Required(ErrorMessage = "Ward is required")]
        public string Ward { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(COD|VNPay)$", ErrorMessage = "Payment method must be COD or VNPay")]
        public string PaymentMethod { get; set; } = "COD";
    }

    // Response DTO for order details
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        
        // Amounts
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        
        // Delivery Info
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string DeliveryAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string? Notes { get; set; }
        
        // Payment
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    // DTO for order items in response
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    // Simple order summary for lists
    public class OrderSummaryDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
