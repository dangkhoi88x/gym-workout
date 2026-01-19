using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.AdminDTOs;
using GymAngel.Business.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAngel.Api.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get overall dashboard statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get revenue chart data
        /// </summary>
        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] int months = 12)
        {
            try
            {
                var data = await _dashboardService.GetRevenueChartDataAsync(months);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve revenue chart", error = ex.Message });
            }
        }

        /// <summary>
        /// Get orders chart data
        /// </summary>
        [HttpGet("orders-chart")]
        public async Task<IActionResult> GetOrdersChart([FromQuery] int months = 12)
        {
            try
            {
                var data = await _dashboardService.GetOrdersChartDataAsync(months);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve orders chart", error = ex.Message });
            }
        }

        /// <summary>
        /// Get recent admin actions
        /// </summary>
        [HttpGet("recent-actions")]
        public async Task<IActionResult> GetRecentActions([FromQuery] int count = 50)
        {
            try
            {
                var actions = await _dashboardService.GetRecentActionsAsync(count);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve actions", error = ex.Message });
            }
        }

        /// <summary>
        /// Log an admin action (internal use)
        /// </summary>
        [HttpPost("log-action")]
        public async Task<IActionResult> LogAction([FromBody] LogActionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var emailClaim = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _dashboardService.LogActionAsync(
                    userId,
                    emailClaim ?? "unknown",
                    request.Action,
                    request.EntityType,
                    request.EntityId,
                    request.Details
                );

                return Ok(new { message = "Action logged successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to log action", error = ex.Message });
            }
        }
    }

    public class LogActionRequest
    {
        public string Action { get; set; } = null!;
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string? Details { get; set; }
    }
}
