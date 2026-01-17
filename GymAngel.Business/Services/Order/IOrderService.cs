using System.Collections.Generic;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.OrderDTOs;

namespace GymAngel.Business.Services.Order
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO dto, int userId);
        Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, int userId);
        Task<List<OrderSummaryDTO>> GetUserOrdersAsync(int userId);
        Task<bool> CancelOrderAsync(int orderId, int userId);
    }
}
