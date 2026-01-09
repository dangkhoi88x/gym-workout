// Modern Brutalist Signup Form JavaScript
class ModernBrutalistSignupForm {
    constructor() {
        this.form = document.getElementById('signupForm');
        this.usernameInput = document.getElementById('username');
        this.nameInput = document.getElementById('name');
        this.emailInput = document.getElementById('email');
        this.passwordInput = document.getElementById('password');
        this.confirmInput = document.getElementById('confirmPassword');
        this.passwordToggle = document.getElementById('passwordToggle');
        this.submitButton = this.form.querySelector('.login-btn');
        this.successMessage = document.getElementById('successMessage');
        this.socialButtons = document.querySelectorAll('.social-btn');
        this.init();
    }

    init() {
        this.bindEvents();
        this.setupPasswordToggle();
    }

    bindEvents() {
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
        
        // Clear errors on input
        this.usernameInput.addEventListener('input', () => this.clearError('username'));
        this.nameInput.addEventListener('input', () => this.clearError('name'));
        this.emailInput.addEventListener('input', () => this.clearError('email'));
        this.passwordInput.addEventListener('input', () => this.clearError('password'));
        this.confirmInput.addEventListener('input', () => this.clearError('confirm'));
    }

    setupPasswordToggle() {
        if (this.passwordToggle) {
            this.passwordToggle.addEventListener('click', () => {
                const type = this.passwordInput.type === 'password' ? 'text' : 'password';
                this.passwordInput.type = type;
                const toggleText = this.passwordToggle.querySelector('.toggle-text');
                if (toggleText) {
                    toggleText.textContent = type === 'password' ? 'SHOW' : 'HIDE';
                }
            });
        }
    }

    validate() {
        let valid = true;
        this.clearErrors();

        if (!this.usernameInput.value.trim()) {
            this.showError('username', 'Username is required');
            valid = false;
        }

        if (!this.nameInput.value.trim()) {
            this.showError('name', 'Full name is required');
            valid = false;
        }

        const email = this.emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) {
            this.showError('email', 'Email is required');
            valid = false;
        } else if (!emailRegex.test(email)) {
            this.showError('email', 'Please enter a valid email address');
            valid = false;
        }

        if (this.passwordInput.value.length < 6) {
            this.showError('password', 'Password must be at least 6 characters');
            valid = false;
        }

        if (this.passwordInput.value !== this.confirmInput.value) {
            this.showError('confirm', 'Passwords do not match');
            valid = false;
        }

        return valid;
    }

    showError(field, message) {
        const errorEl = document.getElementById(`${field}Error`);
        if (errorEl) {
            errorEl.textContent = message;
            errorEl.classList.add('show');
        }
        
        const input = document.getElementById(field === 'confirm' ? 'confirmPassword' : field);
        if (input) {
            const formGroup = input.closest('.form-group');
            if (formGroup) formGroup.classList.add('error');
        }
    }

    clearError(field) {
        const errorEl = document.getElementById(`${field}Error`);
        if (errorEl) {
            errorEl.textContent = '';
            errorEl.classList.remove('show');
        }
        
        const input = document.getElementById(field === 'confirm' ? 'confirmPassword' : field);
        if (input) {
            const formGroup = input.closest('.form-group');
            if (formGroup) formGroup.classList.remove('error');
        }
    }

    clearErrors() {
        document.querySelectorAll('.error-message').forEach(el => {
            el.textContent = '';
            el.classList.remove('show');
        });
        document.querySelectorAll('.form-group').forEach(el => {
            el.classList.remove('error');
        });
    }

    async handleSubmit(e) {
        e.preventDefault();

        if (!this.validate()) return;

        this.setLoading(true);

        // Build request body matching RegisterDTO on backend
        const requestBody = {
            UserName: this.usernameInput.value.trim(),
            Email: this.emailInput.value.trim(),
            FullName: this.nameInput.value.trim(),
            Password: this.passwordInput.value
        };

        try {
            const apiBase = window.__ENV__?.API_BASE || '';
            const res = await fetch(`${apiBase}/api/auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(requestBody)
            });

            const data = await res.json();

            if (res.ok) {
                this.showSuccess();
                setTimeout(() => {
                    window.location.href = 'signin.html';
                }, 2000);
            } else {
                // Handle error from API
                const errorMsg = data.message || data.error || 'Registration failed';
                this.showError('email', errorMsg);
            }
        } catch (err) {
            this.showError('email', `Network error: ${err.message}`);
        } finally {
            this.setLoading(false);
        }
    }

    setLoading(loading) {
        this.submitButton.classList.toggle('loading', loading);
        this.submitButton.disabled = loading;

        // Disable social buttons during loading
        this.socialButtons.forEach(button => {
            button.style.pointerEvents = loading ? 'none' : 'auto';
            button.style.opacity = loading ? '0.6' : '1';
        });
    }

    showSuccess() {
        // Hide form with smooth transition
        this.form.style.transform = 'scale(0.95)';
        this.form.style.opacity = '0';

        setTimeout(() => {
            this.form.style.display = 'none';
            const socialLogin = document.querySelector('.social-login');
            const signupLink = document.querySelector('.signup-link');
            if (socialLogin) socialLogin.style.display = 'none';
            if (signupLink) signupLink.style.display = 'none';

            // Show success message
            this.successMessage.classList.add('show');
        }, 300);
    }
}

// Initialize the form when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new ModernBrutalistSignupForm();
});
