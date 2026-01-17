using GymAngel.Business.DTOs.CartDTOs;
using GymAngel.Data;
using GymAngel.Data.Repositories;
using GymAngel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAngel.Business.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ApplicationDbContext _context;

        public CartService(ICartRepository cartRepository, ApplicationDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        public async Task<CartDTO> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return MapToCartDTO(cart);
        }

        public async Task<CartDTO> AddToCartAsync(int userId, AddToCartDTO dto)
        {
            // Validate product exists
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            // Validate stock availability
            if (product.Quantity < dto.Quantity)
                throw new Exception($"Insufficient stock. Only {product.Quantity} items available.");

            var cart = await GetOrCreateCartAsync(userId);

            // Check if item already exists in cart
            var existingItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, dto.ProductId);

            if (existingItem != null)
            {
                // Check total quantity after adding
                int newQuantity = existingItem.Quantity + dto.Quantity;
                if (product.Quantity < newQuantity)
                    throw new Exception($"Cannot add {dto.Quantity} items. Only {product.Quantity - existingItem.Quantity} more available.");
                
                // Update quantity
                existingItem.Quantity = newQuantity;
                await _cartRepository.UpdateCartItemQuantityAsync(existingItem.Id, existingItem.Quantity);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            await _cartRepository.UpdateCartAsync(cart);

            // Reload cart with updated data
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task<CartDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, dto.ProductId);

            if (item == null)
                throw new Exception("Item not found in cart");

            // Validate stock before update
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found");
                
            if (product.Quantity < dto.Quantity)
                throw new Exception($"Insufficient stock. Only {product.Quantity} items available.");

            await _cartRepository.UpdateCartItemQuantityAsync(item.Id, dto.Quantity);
            await _cartRepository.UpdateCartAsync(cart);

            // Reload cart
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task<CartDTO> RemoveFromCartAsync(int userId, int productId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, productId);

            if (item != null)
            {
                await _cartRepository.RemoveCartItemAsync(item.Id);
                await _cartRepository.UpdateCartAsync(cart);
            }

            // Reload cart
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task<CartDTO> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            await _cartRepository.ClearCartItemsAsync(cart.Id);
            await _cartRepository.UpdateCartAsync(cart);

            // Reload cart
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task<CartDTO> SyncCartAsync(int userId, SyncCartDTO dto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            // Merge logic: For each item in LocalStorage
            foreach (var localItem in dto.Items)
            {
                // Validate product exists
                var product = await _context.Products.FindAsync(localItem.ProductId);
                if (product == null)
                    continue; // Skip invalid products

                // Validate stock availability - skip items with insufficient stock
                if (product.Quantity < localItem.Quantity)
                    continue; // Skip items that exceed stock

                var existingItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, localItem.ProductId);

                if (existingItem != null)
                {
                    // Item exists in both: take max quantity, but not more than stock
                    var maxQuantity = Math.Max(existingItem.Quantity, localItem.Quantity);
                    maxQuantity = Math.Min(maxQuantity, product.Quantity); // Cap at available stock
                    await _cartRepository.UpdateCartItemQuantityAsync(existingItem.Id, maxQuantity);
                }
                else
                {
                    // Item only in LocalStorage: add to cart
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = localItem.ProductId,
                        Quantity = localItem.Quantity,
                        UnitPrice = product.Price
                    };
                    await _cartRepository.AddCartItemAsync(cartItem);
                }
            }

            await _cartRepository.UpdateCartAsync(cart);

            // Reload cart with all merged items
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        // Helper methods
        private async Task<Domain.Entities.Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            
            if (cart == null)
            {
                cart = new Domain.Entities.Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                cart = await _cartRepository.CreateCartAsync(cart);
            }

            return cart;
        }

        private CartDTO MapToCartDTO(Domain.Entities.Cart cart)
        {
            return new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.CartItems?.Select(ci => new CartItemDTO
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? string.Empty,
                    ProductImageUrl = ci.Product?.ImageUrl,
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    StockAvailable = ci.Product?.Quantity ?? 0
                }).ToList() ?? new List<CartItemDTO>()
            };
        }
    }
}
