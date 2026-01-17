using GymAngel.Business.DTOs.CartDTOs;
using GymAngel.Business.DTOs.DiscountDTOs;
using GymAngel.Business.Services.Cart;
using GymAngel.Data.Repositories;
using GymAngel.Domain.Entities;

namespace GymAngel.Business.Services.Discount
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountCodeRepository _discountRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartService _cartService;

        public DiscountService(
            IDiscountCodeRepository discountRepository,
            ICartRepository cartRepository,
            ICartService cartService)
        {
            _discountRepository = discountRepository;
            _cartRepository = cartRepository;
            _cartService = cartService;
        }

        public async Task<CartDTO> ApplyDiscountToCartAsync(int userId, string code)
        {
            // Get cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            // Get discount code
            var discountCode = await _discountRepository.GetByCodeAsync(code);
            if (discountCode == null)
                throw new Exception("Invalid discount code");

            // Validate discount code
            ValidateDiscountCode(discountCode, cart);

            // Apply discount to cart
            cart.DiscountCodeId = discountCode.Id;
            await _cartRepository.UpdateCartAsync(cart);

            // Return cart with discount applied
            return await _cartService.GetCartAsync(userId);
        }

        public async Task<CartDTO> RemoveDiscountFromCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            cart.DiscountCodeId = null;
            await _cartRepository.UpdateCartAsync(cart);

            return await _cartService.GetCartAsync(userId);
        }

        private void ValidateDiscountCode(DiscountCode code, Domain.Entities.Cart cart)
        {
            var now = DateTime.UtcNow;

            // Check if code is active
            if (!code.IsActive)
                throw new Exception("This discount code is not active");

            // Check validity period
            if (now < code.ValidFrom)
                throw new Exception("This discount code is not yet valid");

            if (now > code.ValidUntil)
                throw new Exception("This discount code has expired");

            // Check usage limit
            if (code.UsageLimit.HasValue && code.UsedCount >= code.UsageLimit.Value)
                throw new Exception("This discount code has reached its usage limit");

            // Check minimum order amount
            if (code.MinimumOrderAmount.HasValue)
            {
                var cartTotal = cart.CartItems?.Sum(ci => ci.Quantity * ci.UnitPrice) ?? 0;
                if (cartTotal < code.MinimumOrderAmount.Value)
                    throw new Exception($"Minimum order amount is {code.MinimumOrderAmount.Value:N0} VND");
            }
        }

        public decimal CalculateDiscount(decimal subtotal, DiscountCode code)
        {
            if (code.DiscountType == "Percentage")
            {
                return subtotal * (code.DiscountValue / 100);
            }
            else // FixedAmount
            {
                return Math.Min(code.DiscountValue, subtotal); // Don't exceed subtotal
            }
        }
    }
}
