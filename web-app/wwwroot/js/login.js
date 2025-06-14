// Theme switcher
document.addEventListener('DOMContentLoaded', function() {
    const currentTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', currentTheme);
    updateToggleButton(currentTheme);
});

function switchTheme() {
    const currentTheme = document.documentElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    
    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    updateToggleButton(newTheme);
}

function updateToggleButton(theme) {
    const toggleButton = document.querySelector('.theme-toggle');
    toggleButton.textContent = theme === 'dark' ? '☀️ Light' : '🌙 Dark';
}

// Login form handling
document.getElementById('loginForm').addEventListener('submit', function(e) {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const loginBtn = document.getElementById('loginBtn');
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');
    
    // Hide previous messages
    errorMessage.style.display = 'none';
    successMessage.style.display = 'none';
    
    // Show loading state
    loginBtn.classList.add('btn-loading');
    loginBtn.disabled = true;
    
    // Simulate login process
    setTimeout(() => {
        // Simple validation (replace with actual authentication)
        if (email === 'admin@example.com' && password === 'password') {
            // Success
            successMessage.style.display = 'block';
            setTimeout(() => {
                // Redirect to dashboard or home page
                alert('Login successful! Redirecting to dashboard...');
                // window.location.href = '/dashboard';
            }, 1500);
        } else {
            // Error
            errorMessage.style.display = 'block';
            loginBtn.classList.remove('btn-loading');
            loginBtn.disabled = false;
        }
    }, 1500);
});

function showForgotPassword() {
    alert('Forgot password functionality would be implemented here.');
}

function showRegister() {
    alert('Registration page would be implemented here.');
}

// Form validation
document.getElementById('email').addEventListener('blur', function() {
    const email = this.value;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    if (email && !emailRegex.test(email)) {
        this.style.borderColor = 'var(--error-color)';
    } else {
        this.style.borderColor = 'var(--border-color)';
    }
});

document.getElementById('password').addEventListener('input', function() {
    const password = this.value;
    
    if (password.length > 0 && password.length < 6) {
        this.style.borderColor = 'var(--error-color)';
    } else {
        this.style.borderColor = 'var(--border-color)';
    }
});