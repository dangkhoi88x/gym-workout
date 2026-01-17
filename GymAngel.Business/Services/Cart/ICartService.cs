using GymAngel.Business.DTOs.CartDTOs;

namespace GymAngel.Business.Services.Cart
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(int userId);
        Task<CartDTO> AddToCartAsync(int userId, AddToCartDTO dto);
        Task<CartDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO dto);
        Task<CartDTO> RemoveFromCartAsync(int userId, int productId);
        Task<CartDTO> ClearCartAsync(int userId);
        Task<CartDTO> SyncCartAsync(int userId, SyncCartDTO dto);
    }
}
