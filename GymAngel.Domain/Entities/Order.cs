using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymAngel.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!; // ORD-YYYYMMDD-XXXX
        public int UserId { get; set; } // Foreign key to User (Identity uses int)
        
        // Order Info
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        
        // Amounts
        public decimal SubtotalAmount { get; set; } // Before discount
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; } // After discount = SubtotalAmount - DiscountAmount
        
        // Delivery Information
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string DeliveryAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string? Notes { get; set; }
        
        // Payment
        public int? DiscountCodeId { get; set; }
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        public string PaymentMethod { get; set; } = "COD"; // COD, VNPay
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<OrderItem>? OrderItems { get; set; }
        public DiscountCode? DiscountCode { get; set; }
    }
}

