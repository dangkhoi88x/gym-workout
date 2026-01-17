using System;

namespace GymAngel.Domain.Entities
{
    public class MembershipTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MembershipPlanId { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "COD"; // COD, VNPay
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Suspended
        
        // Auto-Renewal Fields
        public bool AutoRenewal { get; set; } = true; // Default: enabled
        public int RenewalAttempts { get; set; } = 0;
        public DateTime? NextRenewalDate { get; set; }
        public DateTime? LastRenewalAttempt { get; set; }
        
        // Cancellation Fields
        public DateTime? CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        
        // Grace Period
        public DateTime? GracePeriodStart { get; set; }
        public DateTime? GracePeriodEnd { get; set; }
        public bool IsInGracePeriod { get; set; } = false;
        
        // Navigation
        public User User { get; set; } = null!;
        public MembershipPlan MembershipPlan { get; set; } = null!;
    }
}
