using System;

namespace GymAngel.Domain.Entities
{
    public class MembershipPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // "1 Month", "3 Months", etc.
        public string Description { get; set; } = null!;
        public int DurationMonths { get; set; } // 1, 3, 6, 12
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; } // For showing discount
        public bool IsPopular { get; set; } = false; // Highlight popular plans
        public bool IsActive { get; set; } = true;
        public string Features { get; set; } = null!; // JSON array of features
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public ICollection<MembershipTransaction>? MembershipTransactions { get; set; }
    }
}
