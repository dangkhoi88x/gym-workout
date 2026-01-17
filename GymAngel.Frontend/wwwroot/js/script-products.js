// Products Page JavaScript
(function () {
    'use strict';

    // Configuration
    const API_BASE = window.__ENV__?.API_BASE || '';

    if (!API_BASE) {
        console.error('env.js not loaded or API_BASE missing');
    }

    // State
    let allProducts = [];
    let filteredProducts = [];

    // DOM Elements
    const productsContainer = document.getElementById('products-container');
    const loadingState = document.getElementById('loading-state');
    const errorState = document.getElementById('error-state');
    const emptyState = document.getElementById('empty-state');
    const productCount = document.getElementById('product-count');
    const sortSelect = document.getElementById('sort-select');

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        fetchProducts();
        setupEventListeners();
    });

    // Setup event listeners
    function setupEventListeners() {
        sortSelect.addEventListener('change', handleSort);
    }

    // Fetch products from API
    async function fetchProducts() {
        try {
            showState('loading');

            const response = await fetch(`${API_BASE}/api/products`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            allProducts = data;
            filteredProducts = [...allProducts];

            if (allProducts.length === 0) {
                showState('empty');
            } else {
                renderProducts(filteredProducts);
                updateProductCount(filteredProducts.length);
                showState('products');
            }

        } catch (error) {
            console.error('Error fetching products:', error);
            showState('error');
        }
    }

    // Render products grid
    function renderProducts(products) {
        productsContainer.innerHTML = products.map(product => createProductCard(product)).join('');

        // Add animation delay
        const cards = productsContainer.querySelectorAll('.product-card');
        cards.forEach((card, index) => {
            card.style.animationDelay = `${index * 0.1}s`;
        });
    }

    // Create product card HTML
    function createProductCard(product) {
        const isInStock = product.quantity > 0;
        const formattedPrice = formatPrice(product.price);
        const stockText = isInStock
            ? `${product.quantity} in stock`
            : 'Out of stock';
        const stockClass = isInStock ? 'in-stock' : 'out-of-stock';

        return `
            <div class="col-lg-3 col-md-4 col-sm-6">
                <div class="product-card">
                    <div class="product-image">
                        <img src="${product.imageUrl || 'img/portfolio-1.jpg'}" 
                             alt="${product.name}"
                             onerror="this.src='img/portfolio-1.jpg'">
                        ${!isInStock ? '<span class="product-badge out-of-stock-badge">Out of Stock</span>' : ''}
                    </div>
                    <div class="product-body">
                        <div class="product-category">Supplement</div>
                        <h5 class="product-title">${product.name}</h5>
                        <p class="product-description">${product.description || 'No description available'}</p>
                        <div class="product-info">
                            <div class="product-price">${formattedPrice}</div>
                            <div class="product-stock ${stockClass}">${stockText}</div>
                        </div>
                        <a href="product-detail.html?id=${product.id}" class="btn btn-add-cart" ${!isInStock ? 'style="pointer-events: none; opacity: 0.5;"' : ''}>
                            <i class="${isInStock ? 'fa fa-eye' : 'fa fa-times'} me-2"></i>
                            ${isInStock ? 'View Details' : 'Unavailable'}
                        </a>
                    </div>
                </div>
            </div>
        `;
    }

    // Format price to Vietnamese Dong
    function formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

    // Handle sorting
    function handleSort(e) {
        const sortType = e.target.value;

        switch (sortType) {
            case 'price-low':
                filteredProducts.sort((a, b) => a.price - b.price);
                break;
            case 'price-high':
                filteredProducts.sort((a, b) => b.price - a.price);
                break;
            case 'name':
                filteredProducts.sort((a, b) => a.name.localeCompare(b.name));
                break;
            default:
                filteredProducts = [...allProducts];
        }

        renderProducts(filteredProducts);
    }

    // Update product count
    function updateProductCount(count) {
        productCount.textContent = `Showing ${count} products`;
    }

    // Show different states
    function showState(state) {
        loadingState.classList.add('d-none');
        errorState.classList.add('d-none');
        emptyState.classList.add('d-none');
        productsContainer.classList.remove('d-none');

        switch (state) {
            case 'loading':
                loadingState.classList.remove('d-none');
                productsContainer.classList.add('d-none');
                break;
            case 'error':
                errorState.classList.remove('d-none');
                productsContainer.classList.add('d-none');
                break;
            case 'empty':
                emptyState.classList.remove('d-none');
                productsContainer.classList.add('d-none');
                break;
            case 'products':
                // Products container already visible
                break;
        }
    }

})();
