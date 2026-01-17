using GymAngel.Business.DTOs.CartDTOs;

namespace GymAngel.Business.Services.Discount
{
    public interface IDiscountService
    {
        Task<CartDTO> ApplyDiscountToCartAsync(int userId, string code);
        Task<CartDTO> RemoveDiscountFromCartAsync(int userId);
    }
}
