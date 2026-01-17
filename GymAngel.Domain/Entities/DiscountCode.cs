using System;

namespace GymAngel.Domain.Entities
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // e.g., "WELCOME10"
        
        public string DiscountType { get; set; } = "Percentage"; // Percentage or FixedAmount
        public decimal DiscountValue { get; set; } // 10 (for 10%) or 50000 (for 50k VND)
        
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int? UsageLimit { get; set; } // null = unlimited
        public int UsedCount { get; set; } = 0;
        
        public decimal? MinimumOrderAmount { get; set; } // null = no minimum
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
