// Pricing Page JavaScript
(function () {
    'use strict';

    const API_BASE = window.__ENV__.API_BASE;
    const loadingState = document.getElementById('loading-state');
    const pricingGrid = document.getElementById('pricing-grid');
    const paymentModal = document.getElementById('paymentModal');
    const modalContent = document.getElementById('modal-content');

    let selectedPlan = null;
    let selectedPaymentMethod = 'COD';

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        loadPlans();
    });

    // Load membership plans
    async function loadPlans() {
        try {
            showLoading();

            const response = await fetch(`${API_BASE}/api/memberships`);

            if (!response.ok) {
                throw new Error('Failed to load plans');
            }

            const plans = await response.json();
            renderPlans(plans);
            showPlans();
        } catch (error) {
            console.error('Error loading plans:', error);
            showError();
        }
    }

    // Render pricing plans
    function renderPlans(plans) {
        pricingGrid.innerHTML = plans.map(plan => createPlanCard(plan)).join('');
    }

    // Create plan card HTML
    function createPlanCard(plan) {
        const popularClass = plan.isPopular ? 'popular' : '';
        const hasDiscount = plan.discountPercentage && plan.discountPercentage > 0;

        return `
            <div class="pricing-card ${popularClass}">
                <div class="plan-name">${plan.name}</div>
                <div class="plan-description">${plan.description}</div>
                
                <div class="plan-price">
                    ${hasDiscount ? `
                        <div class="original-price">${formatPrice(plan.originalPrice)}</div>
                    ` : ''}
                    <div class="current-price">
                        ${formatPriceShort(plan.price)}
                        <span class="price-currency">VNĐ</span>
                    </div>
                    <div class="plan-duration">for ${plan.durationMonths} month${plan.durationMonths > 1 ? 's' : ''}</div>
                    ${hasDiscount ? `
                        <div class="discount-badge">Save ${plan.discountPercentage}%</div>
                    ` : ''}
                </div>
                
                <ul class="features-list">
                    ${plan.features.map(feature => `
                        <li>
                            <i class="fa fa-check-circle"></i>
                            ${feature}
                        </li>
                    `).join('')}
                </ul>
                
                <button class="subscribe-btn" onclick="handleSubscribe(${plan.id}, '${plan.name}', ${plan.price})">
                    <i class="fa fa-star me-2"></i>
                    Subscribe Now
                </button>
            </div>
        `;
    }

    // Handle subscribe button click
    window.handleSubscribe = function (planId, planName, price) {
        // Check authentication
        if (!isAuthenticated()) {
            window.location.href = `signin.html?redirect=pricing.html`;
            return;
        }

        selectedPlan = { id: planId, name: planName, price: price };
        showPaymentModal();
    };

    // Show payment modal
    function showPaymentModal() {
        modalContent.innerHTML = `
            <h4 class="mb-3">Selected Plan: ${selectedPlan.name}</h4>
            
            <div class="payment-summary">
                <div class="summary-row">
                    <span>Plan Price:</span>
                    <span>${formatPrice(selectedPlan.price)}</span>
                </div>
                <div class="summary-row">
                    <span>Duration:</span>
                    <span>${selectedPlan.name}</span>
                </div>
                <div class="summary-row">
                    <span>Total Amount:</span>
                    <span>${formatPrice(selectedPlan.price)}</span>
                </div>
            </div>
            
            <div class="payment-method">
                <h5 class="mb-3">Payment Method:</h5>
                <div class="method-option selected" onclick="selectPaymentMethod('COD')">
                    <input type="radio" name="payment" value="COD" checked>
                    <strong class="ms-2">Cash on Delivery (COD)</strong>
                    <p class="mb-0 mt-2 text-muted" style="font-size: 0.85rem;">
                        Pay when you visit the gym
                    </p>
                </div>
                <div class="method-option" onclick="selectPaymentMethod('VNPay')" style="opacity: 0.5; cursor: not-allowed;">
                    <input type="radio" name="payment" value="VNPay" disabled>
                    <strong class="ms-2">VNPay</strong>
                    <p class="mb-0 mt-2 text-muted" style="font-size: 0.85rem;">
                        Coming soon
                    </p>
                </div>
            </div>
            
            <button class="confirm-btn" onclick="confirmSubscription()">
                <i class="fa fa-check me-2"></i>
                Confirm Subscription
            </button>
        `;

        paymentModal.style.display = 'block';
    }

    // Select payment method
    window.selectPaymentMethod = function (method) {
        if (method === 'VNPay') return; // Disabled for now

        selectedPaymentMethod = method;

        // Update UI
        document.querySelectorAll('.method-option').forEach(option => {
            option.classList.remove('selected');
        });
        event.target.closest('.method-option').classList.add('selected');
    };

    // Confirm subscription
    window.confirmSubscription = async function () {
        try {
            const confirmBtn = document.querySelector('.confirm-btn');
            confirmBtn.disabled = true;
            confirmBtn.innerHTML = '<i class="fa fa-spinner fa-spin me-2"></i>Processing...';

            const token = getToken();
            const response = await fetch(`${API_BASE}/api/memberships/subscribe`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    planId: selectedPlan.id,
                    paymentMethod: selectedPaymentMethod
                })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Failed to subscribe');
            }

            const result = await response.json();

            // Show success and redirect
            CartUtils.showToast('Subscription successful! 🎉', 'success');

            setTimeout(() => {
                window.location.href = 'profile.html';
            }, 1500);

        } catch (error) {
            console.error('Error subscribing:', error);
            CartUtils.showToast(error.message || 'Failed to subscribe', 'error');

            const confirmBtn = document.querySelector('.confirm-btn');
            confirmBtn.disabled = false;
            confirmBtn.innerHTML = '<i class="fa fa-check me-2"></i>Confirm Subscription';
        }
    };

    // Close modal
    window.closePaymentModal = function () {
        paymentModal.style.display = 'none';
        selectedPlan = null;
        selectedPaymentMethod = 'COD';
    };

    // Close modal when clicking outside
    window.onclick = function (event) {
        if (event.target === paymentModal) {
            closePaymentModal();
        }
    };

    // Helper functions
    function showLoading() {
        loadingState.classList.remove('d-none');
        pricingGrid.classList.add('d-none');
    }

    function showPlans() {
        loadingState.classList.add('d-none');
        pricingGrid.classList.remove('d-none');
    }

    function showError() {
        loadingState.innerHTML = `
            <i class="fa fa-exclamation-triangle fa-3x text-warning mb-3"></i>
            <p class="text-white">Failed to load membership plans</p>
            <button class="btn btn-primary" onclick="location.reload()">Retry</button>
        `;
    }

    function formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

    function formatPriceShort(price) {
        // Format as 500K, 1.2M, etc.
        if (price >= 1000000) {
            return (price / 1000000).toFixed(1) + 'M';
        } else {
            return (price / 1000) + 'K';
        }
    }

})();
