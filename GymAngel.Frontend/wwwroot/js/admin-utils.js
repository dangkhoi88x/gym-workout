// Admin Utilities and Helpers
(function () {
    'use strict';

    window.AdminUtils = {
        // Get admin token
        getToken: function () {
            return localStorage.getItem('gym angel_admin_token');
        },

        // Get admin email
        getEmail: function () {
            return localStorage.getItem('gymangel_admin_email');
        },

        // Check if admin is logged in
        isAuthenticated: function () {
            return !!this.getToken();
        },

        // Redirect to login if not authenticated
        requireAuth: function () {
            if (!this.isAuthenticated()) {
                window.location.href = 'admin-login.html';
                return false;
            }
            return true;
        },

        // Logout
        logout: function () {
            localStorage.removeItem('gymangel_admin_token');
            localStorage.removeItem('gymangel_admin_email');
            window.location.href = 'admin-login.html';
        },

        // API request with auth
        apiRequest: async function (endpoint, options = {}) {
            const token = this.getToken();
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers
            };

            if (token) {
                headers['Authorization'] = `Bearer ${token}`;
            }

            const response = await fetch(`${window.__ENV__.API_BASE}${endpoint}`, {
                ...options,
                headers
            });

            if (response.status === 401) {
                this.logout();
                throw new Error('Unauthorized');
            }

            return response;
        },

        // Toast notification
        showToast: function (message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `toast toast-${type}`;
            toast.innerHTML = `
                <i class="fa fa-${this.getToastIcon(type)}"></i>
                <span>${message}</span>
            `;

            toast.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: ${this.getToastColor(type)};
                color: white;
                padding: 15px 20px;
                border-radius: 8px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.3);
                z-index: 10000;
                animation: slideIn 0.3s ease;
            `;

            document.body.appendChild(toast);

            setTimeout(() => {
                toast.style.animation = 'slideOut 0.3s ease';
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        },

        getToastIcon: function (type) {
            const icons = {
                success: 'check-circle',
                error: 'exclamation-circle',
                warning: 'exclamation-triangle',
                info: 'info-circle'
            };
            return icons[type] || 'info-circle';
        },

        getToastColor: function (type) {
            const colors = {
                success: '#4CAF50',
                error: '#F44336',
                warning: '#FFA500',
                info: '#2196F3'
            };
            return colors[type] || '#2196F3';
        },

        // Format currency
        formatCurrency: function (amount) {
            return new Intl.NumberFormat('vi-VN', {
                style: 'currency',
                currency: 'VND'
            }).format(amount);
        },

        // Format date
        formatDate: function (dateString) {
            if (!dateString) return 'N/A';
            const date = new Date(dateString);
            return date.toLocaleDateString('vi-VN', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        },

        //Format date time
        formatDateTime: function (dateString) {
            if (!dateString) return 'N/A';
            const date = new Date(dateString);
            return date.toLocaleString('vi-VN', {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });
        },

        // Confirm dialog
        confirm: async function (message) {
            return window.confirm(message);
        },

        // Loading state
        setLoading: function (element, loading = true) {
            if (loading) {
                element.disabled = true;
                element.dataset.originalText = element.innerHTML;
                element.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Loading...';
            } else {
                element.disabled = false;
                element.innerHTML = element.dataset.originalText || 'Submit';
            }
        },

        // Status badge HTML
        getStatusBadge: function (status) {
            const badges = {
                'Active': '<span class="badge badge-success">Active</span>',
                'Pending': '<span class="badge badge-warning">Pending</span>',
                'Completed': '<span class="badge badge-success">Completed</span>',
                'Cancelled': '<span class="badge badge-danger">Cancelled</span>',
                'Expired': '<span class="badge badge-danger">Expired</span>',
                'Suspended': '<span class="badge badge-danger">Suspended</span>',
                'Processing': '<span class="badge badge-info">Processing</span>',
                'Shipped': '<span class="badge badge-info">Shipped</span>',
                'Delivered': '<span class="badge badge-success">Delivered</span>'
            };
            return badges[status] || `<span class="badge badge-info">${status}</span>`;
        },

        // Pagination helper
        renderPagination: function (currentPage, totalPages, onPageChange) {
            let html = '<div class="pagination">';

            // Previous
            if (currentPage > 1) {
                html += `<button class="page-btn" onclick="${onPageChange}(${currentPage - 1})">
                    <i class="fa fa-chevron-left"></i>
                </button>`;
            }

            // Pages
            for (let i = 1; i <= totalPages; i++) {
                if (i === currentPage || i === 1 || i === totalPages || Math.abs(i - currentPage) <= 1) {
                    html += `<button class="page-btn ${i === currentPage ? 'active' : ''}" 
                        onclick="${onPageChange}(${i})">${i}</button>`;
                } else if (i === currentPage - 2 || i === currentPage + 2) {
                    html += '<span class="page-ellipsis">...</span>';
                }
            }

            // Next
            if (currentPage < totalPages) {
                html += `<button class="page-btn" onclick="${onPageChange}(${currentPage + 1})">
                    <i class="fa fa-chevron-right"></i>
                </button>`;
            }

            html += '</div>';
            return html;
        }
    };

    // Add CSS animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }

        .pagination {
            display: flex;
            gap: 5px;
            justify-content: center;
            margin-top: 20px;
        }

        .page-btn {
            padding: 8px 12px;
            background: rgba(255, 255, 255, 0.05);
            border: 1px solid rgba(255, 215, 0, 0.2);
            color: #fff;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.3s;
        }

        .page-btn:hover {
            background: rgba(255, 215, 0, 0.2);
            border-color: #FFD700;
        }

        .page-btn.active {
            background: #FFD700;
            color: #000;
            border-color: #FFD700;
        }

        .page-ellipsis {
            padding: 8px;
            color: rgba(255, 255, 255, 0.5);
        }
    `;
    document.head.appendChild(style);

})();
