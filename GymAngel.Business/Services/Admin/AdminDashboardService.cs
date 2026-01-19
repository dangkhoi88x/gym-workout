using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.AdminDTOs;
using GymAngel.Data;
using GymAngel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAngel.Business.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            // Total revenue (all orders)
            var totalRevenue = await _context.Orders
                .Where(o => o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            // Total orders
            var totalOrders = await _context.Orders.CountAsync();

            // Active members
            var activeMembers = await _context.Users
                .Where(u => u.MembershipStatus == true && u.MembershipExpiry > now)
                .CountAsync();

            // Low stock products
            var lowStockCount = await _context.Products
                .Where(p => p.Quantity < 10)
                .CountAsync();

            // Monthly revenue
            var monthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth && o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            // Monthly orders
            var monthlyOrders = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth)
                .CountAsync();

            // New members this month
            var newMembers = await _context.Users
                .Where(u => u.CreatedAt >= startOfMonth)
                .CountAsync();

            // Expiring memberships (30 days)
            var expiringDate = now.AddDays(30);
            var expiringMemberships = await _context.Users
                .Where(u => u.MembershipStatus == true 
                    && u.MembershipExpiry.HasValue 
                    && u.MembershipExpiry.Value <= expiringDate 
                    && u.MembershipExpiry.Value > now)
                .CountAsync();

            // Average order value
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Revenue growth (this month vs last month)
            var lastMonthRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startOfLastMonth && o.OrderDate < startOfMonth && o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount);

            var revenueGrowth = lastMonthRevenue > 0 
                ? ((monthlyRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;

            return new DashboardStatsDTO
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                ActiveMembers = activeMembers,
                LowStockCount = lowStockCount,
                MonthlyRevenue = monthlyRevenue,
                MonthlyOrders = monthlyOrders,
                NewMembersThisMonth = newMembers,
                ExpiringMemberships = expiringMemberships,
                AverageOrderValue = averageOrderValue,
                RevenueGrowth = revenueGrowth
            };
        }

        public async Task<RevenueChartDTO> GetRevenueChartDataAsync(int months = 12)
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddMonths(-months);

            var labels = new List<string>();
            var productRevenue = new List<decimal>();
            var membershipRevenue = new List<decimal>();
            var totalRevenue = new List<decimal>();

            for (int i = months - 1; i >= 0; i--)
            {
                var monthStart = now.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1);
                
                labels.Add(monthStart.ToString("MMM yyyy"));

                // Product revenue
                var prodRev = await _context.Orders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd && o.Status != "Cancelled")
                    .SumAsync(o => o.TotalAmount);
                productRevenue.Add(prodRev);

                // Membership revenue
                var memberRev = await _context.MembershipTransactions
                    .Where(t => t.TransactionDate >= monthStart && t.TransactionDate < monthEnd && t.PaymentStatus != "Failed")
                    .SumAsync(t => t.Amount);
                membershipRevenue.Add(memberRev);

                totalRevenue.Add(prodRev + memberRev);
            }

            return new RevenueChartDTO
            {
                Labels = labels,
                ProductRevenue = productRevenue,
                MembershipRevenue = membershipRevenue,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<OrdersChartDTO> GetOrdersChartDataAsync(int months = 12)
        {
            var now = DateTime.UtcNow;
            var labels = new List<string>();
            var orderCounts = new List<int>();

            for (int i = months - 1; i >= 0; i--)
            {
                var monthStart = now.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1);
                
                labels.Add(monthStart.ToString("MMM yyyy"));

                var count = await _context.Orders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd)
                    .CountAsync();
                orderCounts.Add(count);
            }

            // Orders by status
            var ordersByStatus = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            return new OrdersChartDTO
            {
                Labels = labels,
                OrderCounts = orderCounts,
                OrdersByStatus = ordersByStatus
            };
        }

        public async Task LogActionAsync(int adminUserId, string adminEmail, string action, string? entityType = null, int? entityId = null, string? details = null)
        {
            var log = new AdminActionLog
            {
                AdminUserId = adminUserId,
                AdminEmail = adminEmail,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AdminActionLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdminActionDTO>> GetRecentActionsAsync(int count = 50)
        {
            var logs = await _context.AdminActionLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .Select(l => new AdminActionDTO
                {
                    Id = l.Id,
                    AdminEmail = l.AdminEmail,
                    Action = l.Action,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Details = l.Details,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            return logs;
        }
    }
}
