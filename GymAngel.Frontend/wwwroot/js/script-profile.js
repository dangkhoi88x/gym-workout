// User Profile Page JavaScript
(function () {
    'use strict';

    const API_BASE = window.__ENV__.API_BASE;

    // DOM Elements
    const loadingState = document.getElementById('loading-state');
    const userInfoSection = document.getElementById('user-info-section');
    const membershipSection = document.getElementById('membership-section');
    const historySection = document.getElementById('history-section');
    const userInfoContent = document.getElementById('user-info-content');
    const membershipContent = document.getElementById('membership-content');
    const historyContent = document.getElementById('history-content');

    // Check authentication
    if (!isAuthenticated()) {
        window.location.href = 'signin.html?redirect=profile.html';
        return;
    }

    // Initialize
    document.addEventListener('DOMContentLoaded', () => {
        loadProfile();
    });

    // Load user profile and membership status
    async function loadProfile() {
        try {
            showLoading();

            const token = getToken();

            // Fetch membership status (includes user info in our case)
            const response = await fetch(`${API_BASE}/api/memberships/my-status`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                if (response.status === 401) {
                    logout();
                    window.location.href = 'signin.html?redirect=profile.html';
                    return;
                }
                throw new Error('Failed to load profile');
            }

            const data = await response.json();

            // Also get user info from token/auth
            renderUserInfo();
            renderMembershipStatus(data);
            renderMembershipHistory(data.history || []);

            showContent();
        } catch (error) {
            console.error('Error loading profile:', error);
            showError();
        }
    }

    // Render user information
    function renderUserInfo() {
        // Get user info from localStorage or decoded token
        const userEmail = getCurrentUserEmail(); // from auth.js

        userInfoContent.innerHTML = `
            <div class="info-row">
                <span class="info-label">Email:</span>
                <span class="info-value">${userEmail || 'Not available'}</span>
            </div>
            <div class="info-row">
                <span class="info-label">Member Since:</span>
                <span class="info-value">${formatDate(new Date())}</span>
            </div>
            <div class="info-row">
                <span class="info-label">Account Status:</span>
                <span class="info-value text-success">Active</span>
            </div>
        `;
    }

    // Render membership status
    function renderMembershipStatus(data) {
        if (!data.hasActiveMembership) {
            membershipContent.innerHTML = `
                <div class="text-center py-4">
                    <span class="membership-badge badge-none">No Active Membership</span>
                    <p class="mt-3 mb-4 text-muted">You don't have an active gym membership yet.</p>
                    <a href="pricing.html" class="btn btn-primary px-5 py-3">
                        <i class="fa fa-star me-2"></i>
                        View Membership Plans
                    </a>
                </div>
            `;
            return;
        }

        const isExpiringSoon = data.daysRemaining !== null && data.daysRemaining <= 7;
        const badgeClass = data.daysRemaining > 0 ? 'badge-active' : 'badge-expired';
        const statusText = data.daysRemaining > 0 ? 'Active' : 'Expired';

        membershipContent.innerHTML = `
            <div class="text-center mb-4">
                <span class="membership-badge ${badgeClass}">${statusText}</span>
            </div>
            
            <div class="info-row">
                <span class="info-label">Current Plan:</span>
                <span class="info-value">${data.currentPlanName || 'N/A'}</span>
            </div>
            
            <div class="info-row">
                <span class="info-label">Start Date:</span>
                <span class="info-value">${formatDate(data.membershipStart)}</span>
            </div>
            
            <div class="info-row">
                <span class="info-label">Expiry Date:</span>
                <span class="info-value">${formatDate(data.membershipExpiry)}</span>
            </div>
            
            ${data.daysRemaining !== null && data.daysRemaining >= 0 ? `
                <div class="days-remaining">
                    <div class="d-flex align-items-center justify-content-between">
                        <div>
                            <div class="number">${data.daysRemaining}</div>
                            <div class="text">days remaining</div>
                        </div>
                        ${isExpiringSoon ? `
                            <a href="pricing.html" class="btn btn-warning">
                                <i class="fa fa-refresh me-2"></i>Renew Now
                            </a>
                        ` : ''}
                    </div>
                </div>
            ` : `
                <div class="text-center mt-4">
                    <p class="text-warning mb-3">Your membership has expired</p>
                    <a href="pricing.html" class="btn btn-primary px-5 py-3">
                        <i class="fa fa-refresh me-2"></i>
                        Renew Membership
                    </a>
                </div>
            `}
        `;
    }

    // Render membership history
    function renderMembershipHistory(history) {
        if (!history || history.length === 0) {
            historyContent.innerHTML = `
                <div class="empty-state">
                    <i class="fa fa-history fa-3x mb-3" style="opacity: 0.3;"></i>
                    <p>No membership history yet</p>
                </div>
            `;
            return;
        }

        historyContent.innerHTML = `
            <div class="table-responsive">
                <table class="history-table table table-dark">
                    <thead>
                        <tr>
                            <th>Plan</th>
                            <th>Purchase Date</th>
                            <th>Start - End</th>
                            <th>Amount</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${history.map(item => `
                            <tr>
                                <td>${item.planName}</td>
                                <td>${formatDate(item.transactionDate)}</td>
                                <td>
                                    ${formatDateShort(item.startDate)} - ${formatDateShort(item.expiryDate)}
                                </td>
                                <td>${formatPrice(item.amount)}</td>
                                <td>
                                    <span class="status-tag status-${item.status.toLowerCase()}">${item.status}</span>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;
    }

    // Handle logout
    window.handleLogout = function () {
        if (confirm('Are you sure you want to logout?')) {
            logout();
            window.location.href = 'signin.html';
        }
    };

    // Helper functions
    function showLoading() {
        loadingState.classList.remove('d-none');
        userInfoSection.classList.add('d-none');
        membershipSection.classList.add('d-none');
        historySection.classList.add('d-none');
    }

    function showContent() {
        loadingState.classList.add('d-none');
        userInfoSection.classList.remove('d-none');
        membershipSection.classList.remove('d-none');
        historySection.classList.remove('d-none');
    }

    function showError() {
        loadingState.innerHTML = `
            <i class="fa fa-exclamation-triangle fa-3x text-warning mb-3"></i>
            <p class="text-white">Failed to load profile</p>
            <button class="btn btn-primary" onclick="location.reload()">Retry</button>
        `;
    }

    function getCurrentUserEmail() {
        // Try to get from localStorage if stored during login
        const userInfo = localStorage.getItem('gymangel_user');
        if (userInfo) {
            try {
                return JSON.parse(userInfo).email;
            } catch (e) {
                console.error('Error parsing user info:', e);
            }
        }
        return null;
    }

    function formatDate(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }

    function formatDateShort(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            month: 'short',
            day: 'numeric',
            year: 'numeric'
        });
    }

    function formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

})();
