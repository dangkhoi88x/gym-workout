// Shopping Cart Utility Functions
// Manages cart data in LocalStorage

(function (window) {
    'use strict';

    const CART_STORAGE_KEY = 'gymangel_cart';

    // Cart Utility Object
    const CartUtils = {

        // Helper: Check if user is authenticated
        isAuthenticated: function () {
            return window.isAuthenticated ? window.isAuthenticated() : false;
        },

        // Get cart (hybrid: from API if authenticated, LocalStorage otherwise)
        getCart: async function () {
            if (this.isAuthenticated() && window.CartAPI) {
                try {
                    const cartData = await window.CartAPI.getCart();
                    // Transform API response to LocalStorage format
                    const cart = cartData.items.map(item => ({
                        id: item.productId,
                        name: item.productName,
                        price: item.unitPrice,
                        imageUrl: item.productImageUrl,
                        quantity: item.quantity
                    }));
                    // Update LocalStorage as cache
                    this.saveCartToStorage(cart);
                    return cart;
                } catch (error) {
                    console.error('Error getting cart from API, falling back to LocalStorage:', error);
                    return this.getCartFromStorage();
                }
            } else {
                return this.getCartFromStorage();
            }
        },

        // Get cart from LocalStorage (sync)
        getCartFromStorage: function () {
            try {
                const cart = localStorage.getItem(CART_STORAGE_KEY);
                return cart ? JSON.parse(cart) : [];
            } catch (error) {
                console.error('Error reading cart:', error);
                return [];
            }
        },

        // Save cart to LocalStorage (sync)
        saveCartToStorage: function (cart) {
            try {
                localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));
                this.updateCartBadge();
                return true;
            } catch (error) {
                console.error('Error saving cart:', error);
                return false;
            }
        },

        // Add product to cart (async, uses API if authenticated)
        addToCart: async function (product, quantity = 1) {
            if (this.isAuthenticated() && window.CartAPI) {
                try {
                    const cartData = await window.CartAPI.addToCart(product.id, quantity);
                    // Update LocalStorage cache
                    const cart = cartData.items.map(item => ({
                        id: item.productId,
                        name: item.productName,
                        price: item.unitPrice,
                        imageUrl: item.productImageUrl,
                        quantity: item.quantity
                    }));
                    this.saveCartToStorage(cart);
                    return true;
                } catch (error) {
                    console.error('Error adding to cart via API, falling back to LocalStorage:', error);
                    this.showToast('Failed to sync with server, saved locally', 'warning');
                    return this.addToCartLocal(product, quantity);
                }
            } else {
                return this.addToCartLocal(product, quantity);
            }
        },

        // Add to cart locally (LocalStorage only)
        addToCartLocal: function (product, quantity = 1) {
            const cart = this.getCartFromStorage();
            const existingItem = cart.find(item => item.id === product.id);

            if (existingItem) {
                existingItem.quantity += quantity;
            } else {
                cart.push({
                    id: product.id,
                    name: product.name,
                    price: product.price,
                    imageUrl: product.imageUrl,
                    quantity: quantity,
                    description: product.description
                });
            }

            this.saveCartToStorage(cart);
            return true;
        },

        // Remove product from cart (async)
        removeFromCart: async function (productId) {
            if (this.isAuthenticated() && window.CartAPI) {
                try {
                    const cartData = await window.CartAPI.removeFromCart(productId);
                    const cart = cartData.items.map(item => ({
                        id: item.productId,
                        name: item.productName,
                        price: item.unitPrice,
                        imageUrl: item.productImageUrl,
                        quantity: item.quantity
                    }));
                    this.saveCartToStorage(cart);
                    return true;
                } catch (error) {
                    console.error('Error removing from cart via API:', error);
                    this.showToast('Failed to sync with server', 'error');
                    return this.removeFromCartLocal(productId);
                }
            } else {
                return this.removeFromCartLocal(productId);
            }
        },

        // Remove from cart locally
        removeFromCartLocal: function (productId) {
            let cart = this.getCartFromStorage();
            cart = cart.filter(item => item.id !== productId);
            this.saveCartToStorage(cart);
            return true;
        },

        // Update product quantity (async)
        updateQuantity: async function (productId, quantity) {
            if (this.isAuthenticated() && window.CartAPI) {
                try {
                    if (quantity <= 0) {
                        return await this.removeFromCart(productId);
                    }
                    const cartData = await window.CartAPI.updateCartItem(productId, quantity);
                    const cart = cartData.items.map(item => ({
                        id: item.productId,
                        name: item.productName,
                        price: item.unitPrice,
                        imageUrl: item.productImageUrl,
                        quantity: item.quantity
                    }));
                    this.saveCartToStorage(cart);
                    return true;
                } catch (error) {
                    console.error('Error updating cart via API:', error);
                    this.showToast('Failed to sync with server', 'error');
                    return this.updateQuantityLocal(productId, quantity);
                }
            } else {
                return this.updateQuantityLocal(productId, quantity);
            }
        },

        // Update quantity locally
        updateQuantityLocal: function (productId, quantity) {
            const cart = this.getCartFromStorage();
            const item = cart.find(item => item.id === productId);

            if (item) {
                if (quantity <= 0) {
                    return this.removeFromCartLocal(productId);
                }
                item.quantity = quantity;
                this.saveCartToStorage(cart);
                return true;
            }
            return false;
        },

        // Get cart total (sync, from current storage)
        getCartTotal: function () {
            const cart = this.getCartFromStorage();
            return cart.reduce((total, item) => {
                return total + (item.price * item.quantity);
            }, 0);
        },

        // Get total number of items in cart (sync)
        getCartCount: function () {
            const cart = this.getCartFromStorage();
            return cart.reduce((count, item) => count + item.quantity, 0);
        },

        // Clear entire cart (async)
        clearCart: async function () {
            if (this.isAuthenticated() && window.CartAPI) {
                try {
                    await window.CartAPI.clearCart();
                } catch (error) {
                    console.error('Error clearing cart via API:', error);
                }
            }
            localStorage.removeItem(CART_STORAGE_KEY);
            this.updateCartBadge();
            return true;
        },

        // Check if product is in cart (sync)
        isInCart: function (productId) {
            const cart = this.getCartFromStorage();
            return cart.some(item => item.id === productId);
        },

        // Get product quantity in cart (sync)
        getProductQuantity: function (productId) {
            const cart = this.getCartFromStorage();
            const item = cart.find(item => item.id === productId);
            return item ? item.quantity : 0;
        },

        // Format price to VND
        formatPrice: function (price) {
            return new Intl.NumberFormat('vi-VN', {
                style: 'currency',
                currency: 'VND'
            }).format(price);
        },

        // Update cart badge in navbar
        updateCartBadge: function () {
            const badge = document.getElementById('cart-badge');
            if (badge) {
                const count = this.getCartCount();
                badge.textContent = count;
                badge.style.display = count > 0 ? 'inline-block' : 'none';
            }
        },

        // Show toast notification
        showToast: function (message, type = 'success') {
            // Create toast element
            const toast = document.createElement('div');
            toast.className = `cart-toast cart-toast-${type}`;
            toast.innerHTML = `
                <i class="fa ${type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle'} me-2"></i>
                ${message}
            `;

            document.body.appendChild(toast);

            // Show toast
            setTimeout(() => toast.classList.add('show'), 100);

            // Hide and remove toast
            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        }
    };

    // Expose CartUtils globally
    window.CartUtils = CartUtils;

    // Auto-update badge on page load
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            CartUtils.updateCartBadge();
        });
    } else {
        CartUtils.updateCartBadge();
    }

})(window);
