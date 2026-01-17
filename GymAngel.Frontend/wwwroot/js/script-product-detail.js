// Product Detail Page JavaScript
(function () {
    'use strict';

    const API_BASE = window.__ENV__?.API_BASE || '';

    if (!API_BASE) {
        console.error('env.js not loaded or API_BASE missing');
    }

    let currentProduct = null;
    let quantity = 1;

    // DOM Elements
    const container = document.getElementById('product-detail-container');
    const loadingState = document.getElementById('loading-state');
    const errorState = document.getElementById('error-state');
    const breadcrumb = document.getElementById('breadcrumb-product');

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        const productId = getProductIdFromUrl();
        if (productId) {
            fetchProductDetail(productId);
        } else {
            showError();
        }
    });

    // Get product ID from URL parameter
    function getProductIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get('id');
    }

    // Fetch product details from API
    async function fetchProductDetail(productId) {
        try {
            showState('loading');

            const response = await fetch(`${API_BASE}/api/products/${productId}`);
            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error('Product not found');
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            currentProduct = await response.json();

            renderProduct(currentProduct);
            showState('content');

        } catch (error) {
            console.error('Error fetching product:', error);
            showError();
        }
    }

    // Render product details
    function renderProduct(product) {
        const isInStock = product.quantity > 0;

        breadcrumb.textContent = product.name;
        document.title = `${product.name} - GYM ANGEL`;

        container.innerHTML = `
            <div class="product-detail-row">
                <!-- Product Image -->
                <div class="product-image-section">
                    <div class="product-main-image">
                        <img src="${product.imageUrl || 'img/portfolio-1.jpg'}" 
                             alt="${product.name}"
                             onerror="this.src='img/portfolio-1.jpg'">
                    </div>
                </div>
                
                <!-- Product Info -->
                <div class="product-info-section">
                    <span class="product-category-badge">Supplement</span>
                    <h1 class="product-detail-title">${product.name}</h1>
                    <div class="product-detail-price">${CartUtils.formatPrice(product.price)}</div>
                    
                    <div class="product-stock-info">
                        <span class="stock-badge ${isInStock ? 'in-stock' : 'out-of-stock'}">
                            ${isInStock ? `✓ ${product.quantity} in stock` : '✗ Out of stock'}
                        </span>
                    </div>
                    
                    <div class="product-detail-description">
                        <p>${product.description || 'No description available.'}</p>
                    </div>
                    
                    ${isInStock ? `
                    <div class="quantity-selector-wrapper">
                        <label>Quantity</label>
                        <div class="quantity-selector">
                            <button type="button" class="quantity-btn" id="decrease-btn">−</button>
                            <input type="number" id="quantity-input" class="quantity-input" value="1" min="1" max="${product.quantity}" readonly>
                            <button type="button" class="quantity-btn" id="increase-btn">+</button>
                        </div>
                    </div>
                    ` : ''}
                    
                    <div class="product-actions">
                        <button class="btn-add-to-cart-detail" id="add-to-cart-btn" ${!isInStock ? 'disabled' : ''}>
                            <i class="fa fa-shopping-cart me-2"></i>
                            ${isInStock ? 'Add to Cart' : 'Out of Stock'}
                        </button>
                    </div>
                    
                    <a href="products.html" class="btn-back-to-products">
                        <i class="fa fa-arrow-left"></i>
                        Back to Products
                    </a>
                    
                    <div class="product-meta mt-4">
                        <div class="product-meta-item">
                            <span class="product-meta-label">Product ID:</span>
                            <span class="product-meta-value">#${product.id}</span>
                        </div>
                        <div class="product-meta-item">
                            <span class="product-meta-label">Category:</span>
                            <span class="product-meta-value">Gym Supplement</span>
                        </div>
                        <div class="product-meta-item">
                            <span class="product-meta-label">Availability:</span>
                            <span class="product-meta-value">${isInStock ? 'In Stock' : 'Out of Stock'}</span>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Setup event listeners
        if (isInStock) {
            setupQuantityControls(product.quantity);
            setupAddToCart();
        }
    }

    // Setup quantity controls
    function setupQuantityControls(maxQuantity) {
        const decreaseBtn = document.getElementById('decrease-btn');
        const increaseBtn = document.getElementById('increase-btn');
        const quantityInput = document.getElementById('quantity-input');

        decreaseBtn.addEventListener('click', () => {
            if (quantity > 1) {
                quantity--;
                quantityInput.value = quantity;
                updateQuantityButtons();
            }
        });

        increaseBtn.addEventListener('click', () => {
            if (quantity < maxQuantity) {
                quantity++;
                quantityInput.value = quantity;
                updateQuantityButtons();
            }
        });

        updateQuantityButtons();
    }

    // Update quantity button states
    function updateQuantityButtons() {
        const decreaseBtn = document.getElementById('decrease-btn');
        const increaseBtn = document.getElementById('increase-btn');
        const maxQuantity = currentProduct.quantity;

        decreaseBtn.disabled = quantity <= 1;
        increaseBtn.disabled = quantity >= maxQuantity;
    }

    // Setup add to cart button
    function setupAddToCart() {
        const addToCartBtn = document.getElementById('add-to-cart-btn');

        addToCartBtn.addEventListener('click', () => {
            if (currentProduct) {
                CartUtils.addToCart(currentProduct, quantity);
                CartUtils.showToast(`Added ${quantity}x ${currentProduct.name} to cart!`, 'success');

                // Reset quantity
                quantity = 1;
                document.getElementById('quantity-input').value = 1;
                updateQuantityButtons();
            }
        });
    }

    // Show different states
    function showState(state) {
        loadingState.classList.add('d-none');
        errorState.classList.add('d-none');
        container.classList.add('d-none');

        switch (state) {
            case 'loading':
                loadingState.classList.remove('d-none');
                break;
            case 'error':
                errorState.classList.remove('d-none');
                break;
            case 'content':
                container.classList.remove('d-none');
                break;
        }
    }

    // Show error
    function showError() {
        showState('error');
        breadcrumb.textContent = 'Error';
    }

})();
