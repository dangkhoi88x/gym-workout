using System.Collections.Generic;
using System.Threading.Tasks;
using GymAngel.Domain.Entities;

namespace GymAngel.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<Order> UpdateAsync(Order order);
        Task<string> GenerateOrderNumberAsync();
    }
}
