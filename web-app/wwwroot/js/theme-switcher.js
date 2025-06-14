// Theme Switcher JavaScript
// Add this to your layout or in a separate JS file

document.addEventListener('DOMContentLoaded', function() {
    // Check for saved theme preference or default to light mode
    const currentTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', currentTheme);

    // Create theme toggle button if it doesn't exist
    if (!document.querySelector('.theme-toggle')) {
        createThemeToggleButton();
    }
    
    // Update button text based on current theme
    updateToggleButton(currentTheme);
    
    // Create theme toggle button if it doesn't exist
    if (!document.querySelector('.theme-toggle')) {
        createThemeToggleButton();
    }
    
    // Add event listener to theme toggle button
    document.querySelector('.theme-toggle').addEventListener('click', function() {
        switchTheme();
    });
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
    if (toggleButton) {
        toggleButton.textContent = theme === 'dark' ? '☀️ Light' : '🌙 Dark';
    }
}

function createThemeToggleButton() {
    const toggleButton = document.createElement('button');
    toggleButton.className = 'theme-toggle btn';
    toggleButton.setAttribute('aria-label', 'Toggle theme');
    document.body.appendChild(toggleButton);
}