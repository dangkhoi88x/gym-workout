using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymAngel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAngel.Data.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Plans
        public async Task<List<MembershipPlan>> GetActivePlansAsync()
        {
            return await _context.MembershipPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.DurationMonths)
                .ToListAsync();
        }

        public async Task<MembershipPlan?> GetPlanByIdAsync(int id)
        {
            return await _context.MembershipPlans.FindAsync(id);
        }

        // Transactions
        public async Task<MembershipTransaction> CreateTransactionAsync(MembershipTransaction transaction)
        {
            _context.MembershipTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<MembershipTransaction>> GetUserTransactionsAsync(int userId)
        {
            return await _context.MembershipTransactions
                .Include(t => t.MembershipPlan)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<MembershipTransaction?> GetActiveTransactionAsync(int userId)
        {
            return await _context.MembershipTransactions
                .Include(t => t.MembershipPlan)
                .Where(t => t.UserId == userId && t.Status == "Active")
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();
        }

        public async Task<MembershipTransaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.MembershipTransactions
                .Include(t => t.MembershipPlan)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateTransactionAsync(MembershipTransaction transaction)
        {
            _context.MembershipTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // Expiry management
        public async Task<List<User>> GetExpiredMembershipsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Users
                .Where(u => u.MembershipStatus == true && u.MembershipExpiry < today)
                .ToListAsync();
        }

        public async Task<List<MembershipTransaction>> GetTransactionsForRenewalAsync(DateTime renewalDate)
        {
            return await _context.MembershipTransactions
                .Include(t => t.MembershipPlan)
                .Where(t => t.Status == "Active" 
                    && t.AutoRenewal == true
                    && t.ExpiryDate.Date == renewalDate.Date)
                .ToListAsync();
        }

        public async Task<List<MembershipTransaction>> GetTransactionsExpiringOnAsync(DateTime expiryDate)
        {
            return await _context.MembershipTransactions
                .Include(t => t.MembershipPlan)
                .Where(t => t.Status == "Active" && t.ExpiryDate.Date == expiryDate.Date)
                .ToListAsync();
        }

        public async Task<List<MembershipTransaction>> GetGracePeriodExpirationsAsync(DateTime today)
        {
            return await _context.MembershipTransactions
                .Where(t => t.IsInGracePeriod == true 
                    && t.GracePeriodEnd.HasValue 
                    && t.GracePeriodEnd.Value.Date <= today.Date)
                .ToListAsync();
        }
    }
}
