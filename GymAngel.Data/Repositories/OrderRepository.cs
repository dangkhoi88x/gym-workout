using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymAngel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAngel.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow;
            var datePrefix = today.ToString("yyyyMMdd");
            
            // Get count of orders created today
            var todayOrders = await _context.Orders
                .Where(o => o.OrderDate.Date == today.Date)
                .CountAsync();
            
            var sequence = (todayOrders + 1).ToString("D4");
            return $"ORD-{datePrefix}-{sequence}";
        }
    }
}
