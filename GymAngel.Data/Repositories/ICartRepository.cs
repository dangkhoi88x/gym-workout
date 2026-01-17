using GymAngel.Domain.Entities;

namespace GymAngel.Data.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task<CartItem?> GetCartItemByProductIdAsync(int cartId, int productId);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartItemsAsync(int cartId);
    }
}
