// Shopping Cart Page JavaScript
(function () {
    'use strict';

    // DOM Elements
    const cartItemsContainer = document.getElementById('cart-items-container');
    const cartSummary = document.getElementById('cart-summary');
    const emptyCartState = document.getElementById('empty-cart-state');

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        renderCart();
    });

    // Render cart
    async function renderCart() {
        const cart = await CartUtils.getCart();

        if (cart.length === 0) {
            showEmptyState();
        } else {
            renderCartItems(cart);
            renderCartSummary(cart);
        }
    }

    // Show empty cart state
    function showEmptyState() {
        cartItemsContainer.classList.add('d-none');
        cartSummary.classList.add('d-none');
        emptyCartState.classList.remove('d-none');
    }

    // Render cart items
    function renderCartItems(cart) {
        cartItemsContainer.classList.remove('d-none');
        emptyCartState.classList.add('d-none');

        cartItemsContainer.innerHTML = cart.map(item => createCartItemHtml(item)).join('');

        // Attach event listeners
        attachCartItemListeners();
    }

    // Create cart item HTML
    function createCartItemHtml(item) {
        const subtotal = item.price * item.quantity;

        return `
            <div class="cart-item" data-product-id="${item.id}">
                <div class="cart-item-image">
                    <img src="${item.imageUrl || 'img/portfolio-1.jpg'}" 
                         alt="${item.name}"
                         onerror="this.src='img/portfolio-1.jpg'">
                </div>
                
                <div class="cart-item-details">
                    <h5 class="cart-item-name">${item.name}</h5>
                    <div class="cart-item-price">${CartUtils.formatPrice(item.price)}</div>
                    
                    <div class="cart-item-controls">
                        <div class="quantity-selector-cart">
                            <button class="cart-quantity-btn decrease-btn" data-product-id="${item.id}">−</button>
                            <div class="cart-quantity-value">${item.quantity}</div>
                            <button class="cart-quantity-btn increase-btn" data-product-id="${item.id}">+</button>
                        </div>
                    </div>
                </div>
                
                <div class="cart-item-subtotal">
                    <div class="cart-item-subtotal-label">Subtotal</div>
                    <div class="cart-item-subtotal-value">${CartUtils.formatPrice(subtotal)}</div>
                </div>
                
                <div class="cart-item-remove">
                    <button class="btn-remove-item" data-product-id="${item.id}" title="Remove item">
                        <i class="fa fa-trash"></i>
                    </button>
                </div>
            </div>
        `;
    }

    // Attach event listeners to cart items
    function attachCartItemListeners() {
        // Decrease quantity
        document.querySelectorAll('.decrease-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.currentTarget.dataset.productId);
                const item = CartUtils.getCartFromStorage().find(item => item.id === productId);
                if (item && item.quantity > 1) {
                    CartUtils.updateQuantity(productId, item.quantity - 1);
                    renderCart();
                }
            });
        });

        // Increase quantity
        document.querySelectorAll('.increase-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.currentTarget.dataset.productId);
                const item = CartUtils.getCartFromStorage().find(item => item.id === productId);
                if (item) {
                    CartUtils.updateQuantity(productId, item.quantity + 1);
                    renderCart();
                }
            });
        });

        // Remove item
        document.querySelectorAll('.btn-remove-item').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.currentTarget.dataset.productId);
                if (confirm('Remove this item from cart?')) {
                    CartUtils.removeFromCart(productId);
                    CartUtils.showToast('Item removed from cart', 'success');
                    renderCart();
                }
            });
        });
    }

    // Render cart summary
    function renderCartSummary(cart) {
        cartSummary.classList.remove('d-none');

        const subtotal = CartUtils.getCartTotal();
        const tax = 0; // Set to 0 for now
        const shipping = 0; // Free shipping
        const total = subtotal + tax + shipping;
        const itemCount = CartUtils.getCartCount();

        cartSummary.innerHTML = `
            <h3 class="cart-summary-title">Order Summary</h3>
            
            <div class="cart-summary-row">
                <span class="cart-summary-label">Items (${itemCount}):</span>
                <span class="cart-summary-value">${CartUtils.formatPrice(subtotal)}</span>
            </div>
            
            <div class="cart-summary-row">
                <span class="cart-summary-label">Shipping:</span>
                <span class="cart-summary-value">Free</span>
            </div>
            
            <div class="cart-summary-row">
                <span class="cart-summary-label">Tax:</span>
                <span class="cart-summary-value">${CartUtils.formatPrice(tax)}</span>
            </div>
            
            <div class="cart-summary-row total">
                <span class="cart-summary-label">Total:</span>
                <span class="cart-summary-value">${CartUtils.formatPrice(total)}</span>
            </div>
            
            <div class="cart-summary-actions">
                <button class="btn-checkout" onclick="handleCheckout()">
                    <i class="fa fa-lock me-2"></i>
                    Proceed to Checkout
                </button>
                
                <a href="products.html" class="btn-continue-shopping">
                    <i class="fa fa-arrow-left me-2"></i>
                    Continue Shopping
                </a>
                
                <button class="btn-clear-cart" onclick="handleClearCart()">
                    <i class="fa fa-trash me-2"></i>
                    Clear Cart
                </button>
            </div>
        `;
    }

    // Handle checkout (placeholder)
    window.handleCheckout = function () {
        alert('Checkout functionality will be implemented in the future.\n\nYour order total: ' + CartUtils.formatPrice(CartUtils.getCartTotal()));
    };

    // Handle clear cart
    window.handleClearCart = function () {
        if (confirm('Are you sure you want to clear your entire cart?')) {
            CartUtils.clearCart();
            CartUtils.showToast('Cart cleared', 'success');
            renderCart();
        }
    };

})();
