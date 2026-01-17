using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.MembershipDTOs;
using GymAngel.Data.Repositories;
using GymAngel.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymAngel.Business.Services.Membership
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly UserManager<User> _userManager;

        public MembershipService(IMembershipRepository membershipRepository, UserManager<User> userManager)
        {
            _membershipRepository = membershipRepository;
            _userManager = userManager;
        }

        // Get all active plans
        public async Task<List<MembershipPlanDTO>> GetActivePlansAsync()
        {
            var plans = await _membershipRepository.GetActivePlansAsync();
            
            return plans.Select(p => new MembershipPlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationMonths = p.DurationMonths,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                DiscountPercentage = CalculateDiscount(p.Price, p.OriginalPrice),
                IsPopular = p.IsPopular,
                Features = ParseFeatures(p.Features)
            }).ToList();
        }

        // Get plan by ID
        public async Task<MembershipPlanDTO?> GetPlanByIdAsync(int id)
        {
            var plan = await _membershipRepository.GetPlanByIdAsync(id);
            if (plan == null) return null;

            return new MembershipPlanDTO
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationMonths = plan.DurationMonths,
                Price = plan.Price,
                OriginalPrice = plan.OriginalPrice,
                DiscountPercentage = CalculateDiscount(plan.Price, plan.OriginalPrice),
                IsPopular = plan.IsPopular,
                Features = ParseFeatures(plan.Features)
            };
        }

        // Subscribe to a plan
        public async Task<MembershipSubscriptionResponseDTO> SubscribeToPlanAsync(SubscribeMembershipDTO dto, int userId)
        {
            // 1. Get plan
            var plan = await _membershipRepository.GetPlanByIdAsync(dto.PlanId);
            if (plan == null || !plan.IsActive)
                throw new InvalidOperationException("Invalid or inactive membership plan");

            // 2. Get user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");

            // 3. Calculate dates
            var startDate = DateTime.UtcNow;
            var expiryDate = startDate.AddMonths(plan.DurationMonths);

            // 4. Create transaction
            var transaction = new MembershipTransaction
            {
                UserId = userId,
                MembershipPlanId = plan.Id,
                TransactionDate = DateTime.UtcNow,
                StartDate = startDate,
                ExpiryDate = expiryDate,
                Amount = plan.Price,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentMethod == "COD" ? "Pending" : "Pending",
                Status = "Active"
            };

            await _membershipRepository.CreateTransactionAsync(transaction);

            // 5. Update user membership
            user.MembershipStatus = true;
            user.MembershipStart = startDate;
            user.MembershipExpiry = expiryDate;
            await _userManager.UpdateAsync(user);

            // 6. Return response
            return new MembershipSubscriptionResponseDTO
            {
                TransactionId = transaction.Id,
                PlanId = plan.Id,
                PlanName = plan.Name,
                StartDate = startDate,
                ExpiryDate = expiryDate,
                Amount = plan.Price,
                PaymentMethod = dto.PaymentMethod,
                Status = "Active"
            };
        }

        // Renew membership
        public async Task<MembershipSubscriptionResponseDTO> RenewMembershipAsync(int userId, int planId)
        {
            var dto = new SubscribeMembershipDTO { PlanId = planId, PaymentMethod = "COD" };
            return await SubscribeToPlanAsync(dto, userId);
        }

        // Get user membership status
        public async Task<UserMembershipStatusDTO> GetUserMembershipStatusAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");

            var transactions = await _membershipRepository.GetUserTransactionsAsync(userId);
            var activeTransaction = await _membershipRepository.GetActiveTransactionAsync(userId);

            int? daysRemaining = null;
            if (user.MembershipStatus == true && user.MembershipExpiry.HasValue)
            {
                daysRemaining = Math.Max(0, (user.MembershipExpiry.Value.Date - DateTime.UtcNow.Date).Days);
            }

            return new UserMembershipStatusDTO
            {
                HasActiveMembership = user.MembershipStatus,
                CurrentPlanName = activeTransaction?.MembershipPlan.Name,
                MembershipStart = user.MembershipStart,
                MembershipExpiry = user.MembershipExpiry,
                DaysRemaining = daysRemaining,
                History = transactions.Select(t => new MembershipTransactionDTO
                {
                    Id = t.Id,
                    PlanName = t.MembershipPlan.Name,
                    TransactionDate = t.TransactionDate,
                    StartDate = t.StartDate,
                    ExpiryDate = t.ExpiryDate,
                    Amount = t.Amount,
                    PaymentMethod = t.PaymentMethod,
                    PaymentStatus = t.PaymentStatus,
                    Status = t.Status
                }).ToList()
            };
        }

        // Check and update expired memberships (background job)
        public async Task CheckAndUpdateExpiredMembershipsAsync()
        {
            var expiredUsers = await _membershipRepository.GetExpiredMembershipsAsync();
            
            foreach (var user in expiredUsers)
            {
                user.MembershipStatus = false;
                await _userManager.UpdateAsync(user);

                // Update active transaction status
                var activeTransaction = await _membershipRepository.GetActiveTransactionAsync(user.Id);
                if (activeTransaction != null)
                {
                    activeTransaction.Status = "Expired";
                    await _membershipRepository.UpdateTransactionAsync(activeTransaction);
                }

                // TODO: Send expiry notification email
                Console.WriteLine($"Membership expired for user: {user.Email}");
            }
        }

        // Process auto-renewals (background job - runs daily)
        public async Task ProcessAutoRenewalsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var renewalDate = today.AddDays(3); // 3 days before expiry

            // Find all transactions that need renewal
            var transactionsToRenew = await _membershipRepository.GetTransactionsForRenewalAsync(renewalDate);

            foreach (var transaction in transactionsToRenew)
            {
                if (!transaction.AutoRenewal) continue;

                try
                {
                    await AttemptRenewalAsync(transaction.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Auto-renewal failed for transaction {transaction.Id}: {ex.Message}");
                }
            }
        }

        // Attempt to renew a specific transaction
        public async Task<MembershipSubscriptionResponseDTO> AttemptRenewalAsync(int transactionId)
        {
            var transaction = await _membershipRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            var plan = await _membershipRepository.GetPlanByIdAsync(transaction.MembershipPlanId);
            if (plan == null)
                throw new InvalidOperationException("Plan not found");

            var user = await _userManager.FindByIdAsync(transaction.UserId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Update renewal tracking
            transaction.LastRenewalAttempt = DateTime.UtcNow;
            transaction.RenewalAttempts++;

            try
            {
                // Calculate new dates
                var startDate = transaction.ExpiryDate;
                var expiryDate = startDate.AddMonths(plan.DurationMonths);

                // Create new transaction for renewed period
                var newTransaction = new MembershipTransaction
                {
                    UserId = transaction.UserId,
                    MembershipPlanId = plan.Id,
                    TransactionDate = DateTime.UtcNow,
                    StartDate = startDate,
                    ExpiryDate = expiryDate,
                    Amount = plan.Price,
                    PaymentMethod = transaction.PaymentMethod,
                    PaymentStatus = transaction.PaymentMethod == "COD" ? "Pending" : "Pending",
                    Status = "Active",
                    AutoRenewal = true,
                    NextRenewalDate = expiryDate.AddDays(-3)
                };

                await _membershipRepository.CreateTransactionAsync(newTransaction);

                // Update user membership
                user.MembershipStart = startDate;
                user.MembershipExpiry = expiryDate;
                user.MembershipStatus = true;
                await _userManager.UpdateAsync(user);

                // Mark old transaction as replaced
                transaction.Status = "Renewed";
                await _membershipRepository.UpdateTransactionAsync(transaction);

                // Send success email
                try
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var emailBody = Email.MembershipEmailTemplates.RenewalSuccess(user, newTransaction, plan.Name);
                        await new Auth.EmailService(null!).SendEmailAsync(user.Email, "✓ Membership Renewed Successfully", emailBody);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send renewal email: {ex.Message}");
                }

                return new MembershipSubscriptionResponseDTO
                {
                    TransactionId = newTransaction.Id,
                    PlanId = plan.Id,
                    PlanName = plan.Name,
                    StartDate = startDate,
                    ExpiryDate = expiryDate,
                    Amount = plan.Price,
                    PaymentMethod = newTransaction.PaymentMethod,
                    Status = "Active"
                };
            }
            catch (Exception ex)
            {
                // Update failed attempt
                await _membershipRepository.UpdateTransactionAsync(transaction);
                throw new InvalidOperationException($"Renewal failed: {ex.Message}");
            }
        }

        // Send renewal reminders (background job - runs daily)
        public async Task SendRenewalRemindersAsync()
        {
            var today = DateTime.UtcNow.Date;

            // 30-day reminders
            await SendRemindersByDays(30, "RENEWAL_REMINDER_30");
            
            // 14-day reminders
            await SendRemindersByDays(14, "RENEWAL_REMINDER_14");
            
            // 7-day critical reminders
            await SendRemindersByDays(7, "RENEWAL_REMINDER_7");
        }

        private async Task SendRemindersByDays(int daysBeforeExpiry, string notificationType)
        {
            var targetDate = DateTime.UtcNow.Date.AddDays(daysBeforeExpiry);
            var transactions = await _membershipRepository.GetTransactionsExpiringOnAsync(targetDate);

            foreach (var transaction in transactions)
            {
                if (transaction.Status != "Active") continue;

                var user = await _userManager.FindByIdAsync(transaction.UserId.ToString());
                if (user == null || string.IsNullOrEmpty(user.Email)) continue;

                var plan = await _membershipRepository.GetPlanByIdAsync(transaction.MembershipPlanId);
                if (plan == null) continue;

                try
                {
                    var emailBody = Email.MembershipEmailTemplates.RenewalReminder30Days(user, transaction, plan.Name);
                    var subject = daysBeforeExpiry switch
                    {
                        30 => "Your Gym Angel membership renews in 1 month",
                        14 => "Your membership renews in 2 weeks",
                        7 => "⚠️ Your membership renews in 7 days",
                        _ => "Membership renewal reminder"
                    };

                    await new Auth.EmailService(null!).SendEmailAsync(user.Email, subject, emailBody);
                    Console.WriteLine($"Sent {daysBeforeExpiry}-day reminder to {user.Email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send reminder: {ex.Message}");
                }
            }
        }

        // Process grace period expirations (background job - runs daily)
        public async Task ProcessGracePeriodExpirationsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var transactions = await _membershipRepository.GetGracePeriodExpirationsAsync(today);

            foreach (var transaction in transactions)
            {
                var user = await _userManager.FindByIdAsync(transaction.UserId.ToString());
                if (user == null) continue;

                // Suspend membership
                user.MembershipStatus = false;
                await _userManager.UpdateAsync(user);

                transaction.Status = "Suspended";
                transaction.IsInGracePeriod = false;
                await _membershipRepository.UpdateTransactionAsync(transaction);

                // Send suspension email
                try
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var plan = await _membershipRepository.GetPlanByIdAsync(transaction.MembershipPlanId);
                        var emailBody = Email.MembershipEmailTemplates.MembershipSuspended(user, plan?.Name ?? "Membership");
                        await new Auth.EmailService(null!).SendEmailAsync(user.Email, "Membership Suspended", emailBody);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send suspension email: {ex.Message}");
                }
            }
        }

        // Enable auto-renewal
        public async Task<bool> EnableAutoRenewalAsync(int userId)
        {
            var transaction = await _membershipRepository.GetActiveTransactionAsync(userId);
            if (transaction == null) return false;

            transaction.AutoRenewal = true;
            transaction.NextRenewalDate = transaction.ExpiryDate.AddDays(-3);
            await _membershipRepository.UpdateTransactionAsync(transaction);

            return true;
        }

        // Disable auto-renewal
        public async Task<bool> DisableAutoRenewalAsync(int userId)
        {
            var transaction = await _membershipRepository.GetActiveTransactionAsync(userId);
            if (transaction == null) return false;

            transaction.AutoRenewal = false;
            transaction.NextRenewalDate = null;
            await _membershipRepository.UpdateTransactionAsync(transaction);

            return true;
        }

        // Cancel membership
        public async Task<bool> CancelMembershipAsync(int userId, string? reason)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var transaction = await _membershipRepository.GetActiveTransactionAsync(userId);
            if (transaction == null) return false;

            // Disable auto-renewal
            transaction.AutoRenewal = false;
            transaction.CancellationDate = DateTime.UtcNow;
            transaction.CancellationReason = reason;
            transaction.Status = "Cancelled";
            await _membershipRepository.UpdateTransactionAsync(transaction);

            // Membership remains active until expiry date
            // Don't change user.MembershipStatus yet

            return true;
        }

        // Helper methods
        private int? CalculateDiscount(decimal price, decimal? originalPrice)
        {
            if (!originalPrice.HasValue || originalPrice.Value <= price)
                return null;

            return (int)Math.Round(((originalPrice.Value - price) / originalPrice.Value) * 100);
        }

        private List<string> ParseFeatures(string featuresJson)
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(featuresJson) ?? new List<string>();
            }
            catch
            {
                // Fallback to comma-separated
                return featuresJson?.Split(',').Select(f => f.Trim()).ToList() ?? new List<string>();
            }
        }
    }
}
