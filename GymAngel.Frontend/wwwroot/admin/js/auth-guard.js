// Authentication Guard for Admin Pages
// This script checks if user is authenticated and has admin role
// Include this in all admin pages

(function () {
    const API_BASE_URL = 'http://localhost:5001/api'; // Adjust port if needed

    function getToken() {
        return localStorage.getItem('token');
    }

    function decodeJWT(token) {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        } catch (e) {
            return null;
        }
    }

    function checkAdminAccess() {
        const token = getToken();

        if (!token) {
            // No token, redirect to signin
            window.location.href = '/signin.html?redirect=' + encodeURIComponent(window.location.pathname);
            return false;
        }

        const decoded = decodeJWT(token);

        if (!decoded) {
            // Invalid token, clear and redirect
            localStorage.removeItem('token');
            window.location.href = '/signin.html?redirect=' + encodeURIComponent(window.location.pathname);
            return false;
        }

        // Check if token is expired
        const currentTime = Math.floor(Date.now() / 1000);
        if (decoded.exp && decoded.exp < currentTime) {
            localStorage.removeItem('token');
            alert('Your session has expired. Please login again.');
            window.location.href = '/signin.html?redirect=' + encodeURIComponent(window.location.pathname);
            return false;
        }

        // Check if user has admin role
        const roles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;
        const isAdmin = Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin';

        if (!isAdmin) {
            alert('Access denied. Admin privileges required.');
            window.location.href = '/index.html';
            return false;
        }

        return true;
    }

    // Run check when page loads
    if (!checkAdminAccess()) {
        return;
    }

    // Add token to all API requests
    window.apiHeaders = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getToken()
    };

    // Logout function
    window.adminLogout = function () {
        localStorage.removeItem('token');
        window.location.href = '/signin.html';
    };
})();
