// Cart API Module
// Handles all backend API calls for cart operations

(function (window) {
    'use strict';

    // Import apiFetch from auth.js if available
    const apiFetch = window.apiFetch || async function (path, options) {
        throw new Error('apiFetch not available. Make sure auth.js is loaded.');
    };

    const CartAPI = {
        // GET /api/cart - Get user's cart from backend
        getCart: async function () {
            try {
                const data = await apiFetch('/api/cart', { method: 'GET' });
                return data;
            } catch (error) {
                console.error('Error getting cart from API:', error);
                throw error;
            }
        },

        // POST /api/cart/add - Add product to cart
        addToCart: async function (productId, quantity = 1) {
            try {
                const data = await apiFetch('/api/cart/add', {
                    method: 'POST',
                    body: JSON.stringify({ ProductId: productId, Quantity: quantity })
                });
                return data;
            } catch (error) {
                console.error('Error adding to cart:', error);
                throw error;
            }
        },

        // PUT /api/cart/update - Update cart item quantity
        updateCartItem: async function (productId, quantity) {
            try {
                const data = await apiFetch('/api/cart/update', {
                    method: 'PUT',
                    body: JSON.stringify({ ProductId: productId, Quantity: quantity })
                });
                return data;
            } catch (error) {
                console.error('Error updating cart item:', error);
                throw error;
            }
        },

        // DELETE /api/cart/remove/{productId} - Remove item from cart
        removeFromCart: async function (productId) {
            try {
                const data = await apiFetch(`/api/cart/remove/${productId}`, {
                    method: 'DELETE'
                });
                return data;
            } catch (error) {
                console.error('Error removing from cart:', error);
                throw error;
            }
        },

        // DELETE /api/cart/clear - Clear entire cart
        clearCart: async function () {
            try {
                const data = await apiFetch('/api/cart/clear', {
                    method: 'DELETE'
                });
                return data;
            } catch (error) {
                console.error('Error clearing cart:', error);
                throw error;
            }
        },

        // POST /api/cart/sync - Sync LocalStorage cart with backend
        syncCart: async function (localCartItems) {
            try {
                // Transform cart items to match API DTO
                const items = localCartItems.map(item => ({
                    ProductId: item.id,
                    Quantity: item.quantity
                }));

                const data = await apiFetch('/api/cart/sync', {
                    method: 'POST',
                    body: JSON.stringify({ Items: items })
                });
                return data;
            } catch (error) {
                console.error('Error syncing cart:', error);
                throw error;
            }
        }
    };

    // Expose CartAPI globally
    window.CartAPI = CartAPI;

})(window);
