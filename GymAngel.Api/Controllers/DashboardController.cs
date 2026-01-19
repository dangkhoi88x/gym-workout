using GymAngel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymAngel.Domain.Entities;

namespace GymAngel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get overall dashboard statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                // Total Revenue (sum of all completed/delivered orders)
                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == "Delivered" || o.Status == "Completed")
                    .SumAsync(o => o.TotalAmount);

                // Total Orders
                var totalOrders = await _context.Orders.CountAsync();

                // Orders by Status
                var ordersByStatus = await _context.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                // Total Users
                var totalUsers = await _userManager.Users.CountAsync();

                // Active Memberships (users with valid membership)
                var activeMemberships = await _userManager.Users
                    .Where(u => u.MembershipExpiry != null && u.MembershipExpiry > DateTime.UtcNow)
                    .CountAsync();

                // Recent Revenue Trend (last 7 days)
                var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                var recentRevenueTrend = await _context.Orders
                    .Where(o => o.OrderDate >= sevenDaysAgo && (o.Status == "Delivered" || o.Status == "Completed"))
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.TotalAmount),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                return Ok(new
                {
                    totalRevenue,
                    totalOrders,
                    totalUsers,
                    activeMemberships,
                    ordersByStatus,
                    recentRevenueTrend
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get top 5 selling products
        /// </summary>
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts()
        {
            try
            {
                var topProducts = await _context.OrderItems
                    .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                    .Select(g => new
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        TotalQuantitySold = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                    })
                    .OrderByDescending(p => p.TotalQuantitySold)
                    .Take(5)
                    .ToListAsync();

                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving top products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get revenue data for chart (last N days)
        /// </summary>
        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days).Date;
                
                var revenueData = await _context.Orders
                    .Where(o => o.OrderDate >= startDate && (o.Status == "Delivered" || o.Status == "Completed"))
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // Fill in missing dates with zero revenue
                var allDates = Enumerable.Range(0, days)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                var completeData = allDates.Select(date => new
                {
                    Date = date,
                    Revenue = revenueData.FirstOrDefault(r => r.Date == date)?.Revenue ?? 0
                }).ToList();

                return Ok(completeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving revenue chart data", error = ex.Message });
            }
        }

        /// <summary>
        /// Get order count data for chart (last N days)
        /// </summary>
        [HttpGet("orders-chart")]
        public async Task<IActionResult> GetOrdersChart([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days).Date;
                
                var ordersData = await _context.Orders
                    .Where(o => o.OrderDate >= startDate)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // Fill in missing dates with zero orders
                var allDates = Enumerable.Range(0, days)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                var completeData = allDates.Select(date => new
                {
                    Date = date,
                    OrderCount = ordersData.FirstOrDefault(o => o.Date == date)?.OrderCount ?? 0
                }).ToList();

                return Ok(completeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders chart data", error = ex.Message });
            }
        }
    }
}
