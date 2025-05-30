// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function() {
    const toggle = document.getElementById('darkModeToggle');
    const icon = document.getElementById('darkModeIcon');
    const htmlEl = document.documentElement;
    const current = htmlEl.classList.contains('dark') ? 'dark' : 'light';
    // initialize toggle/icon
    toggle.checked = (current === 'dark');
    icon.textContent = toggle.checked ? '⚪️' : '⚫️';
    toggle.addEventListener('change', () => {
      const theme = toggle.checked ? 'dark' : 'light';
      htmlEl.className = theme;
      localStorage.setItem('theme', theme);
      icon.textContent = theme === 'dark' ? '⚪️' : '⚫️';
    });
  })();