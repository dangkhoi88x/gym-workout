using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymAngel.Business.DTOs.MembershipDTOs
{
    // Request DTO for subscribing to a membership plan
    public class SubscribeMembershipDTO
    {
        [Required]
        public int PlanId { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(COD|VNPay)$", ErrorMessage = "Payment method must be COD or VNPay")]
        public string PaymentMethod { get; set; } = "COD";
    }

    // Response DTO for membership plans
    public class MembershipPlanDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationMonths { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? DiscountPercentage { get; set; } // Calculated
        public bool IsPopular { get; set; }
        public List<string> Features { get; set; } = new();
    }

    // Response DTO for membership subscription
    public class MembershipSubscriptionResponseDTO
    {
        public int TransactionId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    // User's current membership status
    public class UserMembershipStatusDTO
    {
        public bool HasActiveMembership { get; set; }
        public string? CurrentPlanName { get; set; }
        public DateTime? MembershipStart { get; set; }
        public DateTime? MembershipExpiry { get; set; }
        public int? DaysRemaining { get; set; }
        public List<MembershipTransactionDTO> History { get; set; } = new();
    }

    // Membership transaction history
    public class MembershipTransactionDTO
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
