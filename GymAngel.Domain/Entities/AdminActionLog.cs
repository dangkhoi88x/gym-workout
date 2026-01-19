using System;

namespace GymAngel.Domain.Entities
{
    public class AdminActionLog
    {
        public int Id { get; set; }
        public int AdminUserId { get; set; }
        public string AdminEmail { get; set; } = null!;
        
        public string Action { get; set; } = null!; // "CREATE", "UPDATE", "DELETE", "STATUS_CHANGE"
        public string? EntityType { get; set; } // "Product", "Order", "User", "MembershipPlan"
        public int? EntityId { get; set; }
        public string? Details { get; set; } // JSON or description
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public User Admin { get; set; } = null!;
    }
}
