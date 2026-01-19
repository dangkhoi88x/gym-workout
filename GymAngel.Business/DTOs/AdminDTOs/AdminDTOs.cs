using System;

namespace GymAngel.Business.DTOs.AdminDTOs
{
    // Dashboard Statistics
    public class DashboardStatsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int ActiveMembers { get; set; }
        public int LowStockCount { get; set; }
        
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyOrders { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int ExpiringMemberships { get; set; } // Within 30 days
        
        public decimal AverageOrderValue { get; set; }
        public decimal RevenueGrowth { get; set; } // Percentage
    }

    // Revenue Chart Data
    public class RevenueChartDTO
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> ProductRevenue { get; set; } = new();
        public List<decimal> MembershipRevenue { get; set; } = new();
        public List<decimal> TotalRevenue { get; set; } = new();
    }

    // Orders Chart Data
    public class OrdersChartDTO
    {
        public List<string> Labels { get; set; } = new();
        public List<int> OrderCounts { get; set; } = new();
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
    }

    // Order Status Update Request
    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; } = null!;
    }

    // Admin Action Log
    public class AdminActionDTO
    {
        public int Id { get; set; }
        public string AdminEmail { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
