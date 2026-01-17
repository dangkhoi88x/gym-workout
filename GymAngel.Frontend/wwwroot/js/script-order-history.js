// Order History Page JavaScript
(function () {
    'use strict';

    const API_BASE = window.__ENV__.API_BASE;
    const ordersContainer = document.getElementById('orders-container');
    const loadingState = document.getElementById('loading-state');
    const emptyState = document.getElementById('empty-state');
    const orderModal = document.getElementById('orderModal');
    const modalBody = document.getElementById('modal-body');

    // Check authentication
    if (!isAuthenticated()) {
        window.location.href = 'signin.html?redirect=order-history.html';
        return;
    }

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        loadOrders();
    });

    // Load user's orders
    async function loadOrders() {
        try {
            showLoading();

            const token = getToken();
            const response = await fetch(`${API_BASE}/api/orders/user`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                if (response.status === 401) {
                    logout();
                    window.location.href = 'signin.html?redirect=order-history.html';
                    return;
                }
                throw new Error('Failed to load orders');
            }

            const orders = await response.json();

            if (orders.length === 0) {
                showEmpty();
            } else {
                renderOrders(orders);
            }
        } catch (error) {
            console.error('Error loading orders:', error);
            showError();
        }
    }

    // Render orders list
    function renderOrders(orders) {
        ordersContainer.innerHTML = orders.map(order => createOrderCard(order)).join('');
        showOrders();
    }

    // Create order card HTML
    function createOrderCard(order) {
        const statusClass = `status-${order.status.toLowerCase()}`;
        const date = new Date(order.orderDate).toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });

        const canCancel = order.status === 'Pending';

        return `
            <div class="order-card" data-order-id="${order.id}">
                <div class="order-header">
                    <div>
                        <div class="order-number">${order.orderNumber}</div>
                        <small class="text-muted">${date}</small>
                    </div>
                    <span class="order-status ${statusClass}">${order.status}</span>
                </div>
                
                <div class="order-info">
                    <div class="info-item">
                        <i class="fa fa-box"></i>
                        <div>
                            <div class="info-label">Items</div>
                            <div class="info-value">${order.itemCount} item(s)</div>
                        </div>
                    </div>
                    
                    <div class="info-item">
                        <i class="fa fa-money-bill"></i>
                        <div>
                            <div class="info-label">Total Amount</div>
                            <div class="info-value">${formatPrice(order.totalAmount)}</div>
                        </div>
                    </div>
                </div>
                
                <div class="order-actions">
                    <button class="btn-view-details" onclick="viewOrderDetails(${order.id})">
                        <i class="fa fa-eye me-2"></i>View Details
                    </button>
                    ${canCancel ? `
                        <button class="btn-cancel-order" onclick="cancelOrder(${order.id})">
                            <i class="fa fa-times me-2"></i>Cancel Order
                        </button>
                    ` : ''}
                </div>
            </div>
        `;
    }

    // View order details
    window.viewOrderDetails = async function (orderId) {
        try {
            modalBody.innerHTML = '<div class="loading-spinner"><div class="spinner"></div><p class="text-white mt-3">Loading details...</p></div>';
            orderModal.style.display = 'block';

            const token = getToken();
            const response = await fetch(`${API_BASE}/api/orders/${orderId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to load order details');
            }

            const order = await response.json();
            renderOrderDetails(order);
        } catch (error) {
            console.error('Error loading order details:', error);
            modalBody.innerHTML = '<p class="text-danger text-center">Failed to load order details</p>';
        }
    };

    // Render order details in modal
    function renderOrderDetails(order) {
        const date = new Date(order.orderDate).toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });

        const statusClass = `status-${order.status.toLowerCase()}`;

        modalBody.innerHTML = `
            <div class="order-detail">
                <!-- Order Info -->
                <div class="mb-4">
                    <h5 class="text-primary mb-3">Order Information</h5>
                    <div class="row">
                        <div class="col-md-6 mb-2">
                            <strong>Order Number:</strong> ${order.orderNumber}
                        </div>
                        <div class="col-md-6 mb-2">
                            <strong>Order Date:</strong> ${date}
                        </div>
                        <div class="col-md-6 mb-2">
                            <strong>Status:</strong> 
                            <span class="order-status ${statusClass}">${order.status}</span>
                        </div>
                        <div class="col-md-6 mb-2">
                            <strong>Payment Method:</strong> ${order.paymentMethod}
                        </div>
                        <div class="col-md-6 mb-2">
                            <strong>Payment Status:</strong> 
                            <span class="text-${order.paymentStatus === 'Paid' ? 'success' : 'warning'}">${order.paymentStatus}</span>
                        </div>
                    </div>
                </div>

                <!-- Delivery Info -->
                <div class="mb-4">
                    <h5 class="text-primary mb-3">Delivery Information</h5>
                    <div class="row">
                        <div class="col-md-6 mb-2">
                            <strong>Receiver:</strong> ${order.receiverName}
                        </div>
                        <div class="col-md-6 mb-2">
                            <strong>Phone:</strong> ${order.receiverPhone}
                        </div>
                        <div class="col-12 mb-2">
                            <strong>Address:</strong><br>
                            ${order.deliveryAddress}, ${order.ward}, ${order.district}, ${order.city}
                        </div>
                        ${order.notes ? `
                            <div class="col-12 mb-2">
                                <strong>Notes:</strong> ${order.notes}
                            </div>
                        ` : ''}
                    </div>
                </div>

                <!-- Order Items -->
                <div class="mb-4">
                    <h5 class="text-primary mb-3">Order Items</h5>
                    <div class="table-responsive">
                        <table class="table table-dark table-bordered">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th class="text-center">Quantity</th>
                                    <th class="text-end">Unit Price</th>
                                    <th class="text-end">Subtotal</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${order.items.map(item => `
                                    <tr>
                                        <td>${item.productName}</td>
                                        <td class="text-center">${item.quantity}</td>
                                        <td class="text-end">${formatPrice(item.unitPrice)}</td>
                                        <td class="text-end">${formatPrice(item.subtotal)}</td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>
                    </div>
                </div>

                <!-- Order Summary -->
                <div class="text-end">
                    <h5 class="text-primary mb-3">Order Summary</h5>
                    <div class="row justify-content-end">
                        <div class="col-md-6">
                            <div class="d-flex justify-content-between mb-2">
                                <span>Subtotal:</span>
                                <strong>${formatPrice(order.subtotalAmount)}</strong>
                            </div>
                            ${order.discountAmount > 0 ? `
                                <div class="d-flex justify-content-between mb-2 text-success">
                                    <span>Discount:</span>
                                    <strong>-${formatPrice(order.discountAmount)}</strong>
                                </div>
                            ` : ''}
                            <div class="d-flex justify-content-between mb-2">
                                <span>Shipping:</span>
                                <strong>Free</strong>
                            </div>
                            <hr>
                            <div class="d-flex justify-content-between">
                                <h5 class="text-primary">Total:</h5>
                                <h5 class="text-primary">${formatPrice(order.totalAmount)}</h5>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    // Close modal
    window.closeOrderModal = function () {
        orderModal.style.display = 'none';
    };

    // Cancel order
    window.cancelOrder = async function (orderId) {
        if (!confirm('Are you sure you want to cancel this order?')) {
            return;
        }

        try {
            const token = getToken();
            const response = await fetch(`${API_BASE}/api/orders/${orderId}/cancel`, {
                method: 'PATCH',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Failed to cancel order');
            }

            CartUtils.showToast('Order cancelled successfully', 'success');
            loadOrders(); // Reload orders
        } catch (error) {
            console.error('Error cancelling order:', error);
            CartUtils.showToast(error.message || 'Failed to cancel order', 'error');
        }
    };

    // Close modal when clicking outside
    window.onclick = function (event) {
        if (event.target === orderModal) {
            closeOrderModal();
        }
    };

    // Helper functions
    function showLoading() {
        loadingState.classList.remove('d-none');
        ordersContainer.classList.add('d-none');
        emptyState.classList.add('d-none');
    }

    function showOrders() {
        loadingState.classList.add('d-none');
        ordersContainer.classList.remove('d-none');
        emptyState.classList.add('d-none');
    }

    function showEmpty() {
        loadingState.classList.add('d-none');
        ordersContainer.classList.add('d-none');
        emptyState.classList.remove('d-none');
    }

    function showError() {
        loadingState.classList.add('d-none');
        emptyState.innerHTML = `
            <i class="fa fa-exclamation-triangle"></i>
            <h3>Error Loading Orders</h3>
            <p>Failed to load your orders. Please try again.</p>
            <button class="btn btn-primary px-5 py-3" onclick="location.reload()">
                <i class="fa fa-refresh me-2"></i>
                Retry
            </button>
        `;
        emptyState.classList.remove('d-none');
    }

    function formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

})();
