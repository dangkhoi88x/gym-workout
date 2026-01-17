using System.Collections.Generic;
using System.Threading.Tasks;
using GymAngel.Domain.Entities;

namespace GymAngel.Data.Repositories
{
    public interface IMembershipRepository
    {
        // Plans
        Task<List<MembershipPlan>> GetActivePlansAsync();
        Task<MembershipPlan?> GetPlanByIdAsync(int id);
        
        // Transactions
        Task<MembershipTransaction> CreateTransactionAsync(MembershipTransaction transaction);
        Task<List<MembershipTransaction>> GetUserTransactionsAsync(int userId);
        Task<MembershipTransaction?> GetActiveTransactionAsync(int userId);
        Task<MembershipTransaction?> GetTransactionByIdAsync(int id);
        Task UpdateTransactionAsync(MembershipTransaction transaction);
        
        // Expiry management
        Task<List<User>> GetExpiredMembershipsAsync();
        Task<List<MembershipTransaction>> GetTransactionsForRenewalAsync(DateTime renewalDate);
        Task<List<MembershipTransaction>> GetTransactionsExpiringOnAsync(DateTime expiryDate);
        Task<List<MembershipTransaction>> GetGracePeriodExpirationsAsync(DateTime today);
    }
}
