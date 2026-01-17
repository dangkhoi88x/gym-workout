using System.Collections.Generic;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.MembershipDTOs;

namespace GymAngel.Business.Services.Membership
{
    public interface IMembershipService
    {
        // Plans
        Task<List<MembershipPlanDTO>> GetActivePlansAsync();
        Task<MembershipPlanDTO?> GetPlanByIdAsync(int id);
        
        // Subscription
        Task<MembershipSubscriptionResponseDTO> SubscribeToPlanAsync(SubscribeMembershipDTO dto, int userId);
        Task<MembershipSubscriptionResponseDTO> RenewMembershipAsync(int userId, int planId);
        
        // Status
        Task<UserMembershipStatusDTO> GetUserMembershipStatusAsync(int userId);
        
        // Background tasks
        Task CheckAndUpdateExpiredMembershipsAsync();
        Task ProcessAutoRenewalsAsync();
        Task SendRenewalRemindersAsync();
        Task ProcessGracePeriodExpirationsAsync();
        
        // Auto-renewal management
        Task<bool> EnableAutoRenewalAsync(int userId);
        Task<bool> DisableAutoRenewalAsync(int userId);
        Task<MembershipSubscriptionResponseDTO> AttemptRenewalAsync(int transactionId);
        
        // Cancellation
        Task<bool> CancelMembershipAsync(int userId, string? reason);
    }
}
