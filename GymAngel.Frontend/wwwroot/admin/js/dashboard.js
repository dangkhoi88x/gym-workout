// Dashboard Analytics JavaScript
// Fetches data from API and updates the dashboard

const API_BASE_URL = 'http://localhost:5001/api'; // Adjust port if needed

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Format number
function formatNumber(num) {
    return new Intl.NumberFormat('vi-VN').format(num);
}

// Load dashboard statistics
async function loadDashboardStatistics() {
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard/statistics`, {
            headers: window.apiHeaders
        });

        if (!response.ok) {
            throw new Error('Failed to fetch statistics');
        }

        const data = await response.json();

        // Update metric cards
        updateMetricCards(data);

        // Load charts with the data
        loadRevenueChart();
        loadOrdersChart();

        return data;
    } catch (error) {
        console.error('Error loading dashboard statistics:', error);
        showError('Failed to load dashboard statistics');
    }
}

// Update metric cards
function updateMetricCards(data) {
    // Total Revenue
    const revenueElement = document.getElementById('total-revenue');
    if (revenueElement) {
        revenueElement.textContent = formatCurrency(data.totalRevenue);
    }

    // Total Orders
    const ordersElement = document.getElementById('total-orders');
    if (ordersElement) {
        ordersElement.textContent = formatNumber(data.totalOrders);
    }

    // Total Users
    const usersElement = document.getElementById('total-users');
    if (usersElement) {
        usersElement.textContent = formatNumber(data.totalUsers);
    }

    // Active Memberships
    const membershipsElement = document.getElementById('active-memberships');
    if (membershipsElement) {
        membershipsElement.textContent = formatNumber(data.activeMemberships);
    }

    // Orders by Status
    updateOrdersByStatus(data.ordersByStatus);
}

// Update orders by status breakdown
function updateOrdersByStatus(ordersByStatus) {
    const container = document.getElementById('orders-by-status');
    if (!container) return;

    const html = ordersByStatus.map(item => `
        <div class="flex items-center justify-between py-2">
            <span class="text-gray-700 dark:text-gray-300">${item.Status}</span>
            <span class="font-semibold text-gray-900 dark:text-white">${item.Count}</span>
        </div>
    `).join('');

    container.innerHTML = html;
}

// Load revenue chart
async function loadRevenueChart() {
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard/revenue-chart?days=30`, {
            headers: window.apiHeaders
        });

        if (!response.ok) {
            throw new Error('Failed to fetch revenue chart data');
        }

        const data = await response.json();

        // Prepare chart data
        const labels = data.map(item => {
            const date = new Date(item.date);
            return date.toLocaleDateString('vi-VN', { month: 'short', day: 'numeric' });
        });

        const revenues = data.map(item => item.revenue);

        // Create or update chart
        createRevenueChart(labels, revenues);
    } catch (error) {
        console.error('Error loading revenue chart:', error);
    }
}

// Load orders chart
async function loadOrdersChart() {
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard/orders-chart?days=30`, {
            headers: window.apiHeaders
        });

        if (!response.ok) {
            throw new Error('Failed to fetch orders chart data');
        }

        const data = await response.json();

        // Prepare chart data
        const labels = data.map(item => {
            const date = new Date(item.date);
            return date.toLocaleDateString('vi-VN', { month: 'short', day: 'numeric' });
        });

        const orderCounts = data.map(item => item.orderCount);

        // Create or update chart
        createOrdersChart(labels, orderCounts);
    } catch (error) {
        console.error('Error loading orders chart:', error);
    }
}

// Load top products
async function loadTopProducts() {
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard/top-products`, {
            headers: window.apiHeaders
        });

        if (!response.ok) {
            throw new Error('Failed to fetch top products');
        }

        const products = await response.json();

        // Update top products table
        updateTopProductsTable(products);
    } catch (error) {
        console.error('Error loading top products:', error);
    }
}

// Update top products table
function updateTopProductsTable(products) {
    const tbody = document.getElementById('top-products-tbody');
    if (!tbody) return;

    if (products.length === 0) {
        tbody.innerHTML = '<tr><td colspan="4" class="text-center py-4 text-gray-500">No data available</td></tr>';
        return;
    }

    const html = products.map((product, index) => `
        <tr class="border-b border-gray-200 dark:border-gray-700">
            <td class="px-4 py-3 text-gray-900 dark:text-white">${index + 1}</td>
            <td class="px-4 py-3 text-gray-900 dark:text-white">${product.productName}</td>
            <td class="px-4 py-3 text-gray-900 dark:text-white">${formatNumber(product.totalQuantitySold)}</td>
            <td class="px-4 py-3 text-gray-900 dark:text-white">${formatCurrency(product.totalRevenue)}</td>
        </tr>
    `).join('');

    tbody.innerHTML = html;
}

// Create Revenue Chart using Chart.js (will be defined in the HTML)
function createRevenueChart(labels, data) {
    const ctx = document.getElementById('revenueChart');
    if (!ctx) return;

    // Destroy existing chart if it exists
    if (window.revenueChartInstance) {
        window.revenueChartInstance.destroy();
    }

    window.revenueChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Revenue (VND)',
                data: data,
                borderColor: 'rgb(59, 130, 246)',
                backgroundColor: 'rgba(59, 130, 246, 0.1)',
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return 'Revenue: ' + formatCurrency(context.parsed.y);
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return formatCurrency(value);
                        }
                    }
                }
            }
        }
    });
}

// Create Orders Chart using Chart.js
function createOrdersChart(labels, data) {
    const ctx = document.getElementById('ordersChart');
    if (!ctx) return;

    // Destroy existing chart if it exists
    if (window.ordersChartInstance) {
        window.ordersChartInstance.destroy();
    }

    window.ordersChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Orders',
                data: data,
                backgroundColor: 'rgba(16, 185, 129, 0.8)',
                borderColor: 'rgb(16, 185, 129)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            }
        }
    });
}

// Show error message
function showError(message) {
    // You can customize this to show a toast or modal
    console.error(message);
    alert(message);
}

// Initialize dashboard when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    loadDashboardStatistics();
    loadTopProducts();
});
