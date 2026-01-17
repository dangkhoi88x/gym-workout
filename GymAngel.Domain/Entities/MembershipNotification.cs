using System;

namespace GymAngel.Domain.Entities
{
    public class MembershipNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? MembershipTransactionId { get; set; }
        
        public string NotificationType { get; set; } = null!; // RENEWAL_REMINDER_30, RENEWAL_REMINDER_14, etc.
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
        
        public string EmailSubject { get; set; } = null!;
        public string? EmailBody { get; set; }
        public string? ErrorMessage { get; set; }
        
        public int RetryCount { get; set; } = 0;
        public DateTime? LastRetryDate { get; set; }
        
        // Navigation
        public User User { get; set; } = null!;
        public MembershipTransaction? MembershipTransaction { get; set; }
    }
}
