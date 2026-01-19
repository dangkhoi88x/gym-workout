using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.AdminDTOs;

namespace GymAngel.Business.Services.Admin
{
    public interface IAdminDashboardService
    {
        // Dashboard Statistics
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
        Task<RevenueChartDTO> GetRevenueChartDataAsync(int months = 12);
        Task<OrdersChartDTO> GetOrdersChartDataAsync(int months = 12);
        
        // Admin Actions
        Task LogActionAsync(int adminUserId, string adminEmail, string action, string? entityType = null, int? entityId = null, string? details = null);
        Task<List<AdminActionDTO>> GetRecentActionsAsync(int count = 50);
    }
}
