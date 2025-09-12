/**
 * Main JavaScript for Spydersoft Identity Server
 * Handles theme management and general UI functionality
 */

(function() {
    'use strict';

    // Ensure theme system is available
    if (!window.SpydersoftTheme) {
        console.error('SpydersoftTheme not found. Theme functionality may not work properly.');
        return;
    }

    /**
     * Enhanced theme toggle with visual feedback
     */
    function enhanceThemeToggle() {
        const themeButtons = document.querySelectorAll('[onclick*="toggleTheme"], .theme-toggle, [data-toggle-theme]');
        
        themeButtons.forEach(function(button) {
            // Add visual feedback
            button.addEventListener('click', function() {
                // Add a quick pulse animation
                this.style.transform = 'scale(0.95)';
                setTimeout(() => {
                    this.style.transform = 'scale(1)';
                }, 150);
            });
        });
    }

    /**
     * Update theme-dependent elements
     */
    function updateThemeElements(theme) {
        // Update any theme-dependent icons or content
        const themeToggleButtons = document.querySelectorAll('[onclick*="toggleTheme"], .theme-toggle');
        
        themeToggleButtons.forEach(function(button) {
            const icon = button.querySelector('svg');
            if (icon && theme === 'dark') {
                button.setAttribute('title', 'Switch to light mode');
            } else if (icon) {
                button.setAttribute('title', 'Switch to dark mode');
            }
        });
    }

    /**
     * Initialize drawer functionality
     */
    function initializeDrawer() {
        const drawerToggle = document.getElementById('main-drawer');
        const drawerOverlay = document.querySelector('.drawer-overlay');
        
        if (drawerOverlay) {
            drawerOverlay.addEventListener('click', function() {
                if (drawerToggle) {
                    drawerToggle.checked = false;
                }
            });
        }

        // Close drawer when clicking on navigation links on mobile
        const navLinks = document.querySelectorAll('.drawer-side a[href]');
        navLinks.forEach(function(link) {
            link.addEventListener('click', function() {
                // Only close on mobile
                if (window.innerWidth < 1024 && drawerToggle) {
                    setTimeout(() => {
                        drawerToggle.checked = false;
                    }, 100);
                }
            });
        });
    }

    /**
     * Initialize dropdown functionality
     */
    function initializeDropdowns() {
        // Close dropdowns when clicking outside
        document.addEventListener('click', function(event) {
            const dropdowns = document.querySelectorAll('.dropdown');
            
            dropdowns.forEach(function(dropdown) {
                if (!dropdown.contains(event.target)) {
                    const details = dropdown.querySelector('details');
                    if (details) {
                        details.removeAttribute('open');
                    }
                    
                    // Remove focus from dropdown triggers
                    const triggers = dropdown.querySelectorAll('[tabindex]');
                    triggers.forEach(trigger => trigger.blur());
                }
            });
        });
    }

    /**
     * Handle form enhancements
     */
    function enhanceForms() {
        // Add loading states to submit buttons
        const forms = document.querySelectorAll('form');
        
        forms.forEach(function(form) {
            form.addEventListener('submit', function() {
                const submitButtons = form.querySelectorAll('button[type="submit"], input[type="submit"]');
                
                submitButtons.forEach(function(button) {
                    if (button.tagName === 'BUTTON') {
                        button.disabled = true;
                        const originalText = button.innerHTML;
                        button.innerHTML = '<span class="loading loading-spinner loading-sm mr-2"></span>Saving...';
                        
                        // Restore button after a timeout as fallback
                        setTimeout(() => {
                            button.disabled = false;
                            button.innerHTML = originalText;
                        }, 10000);
                    }
                });
            });
        });
    }

    /**
     * Initialize tooltip functionality
     */
    function initializeTooltips() {
        // Simple tooltip implementation for elements with title attributes
        const elementsWithTitles = document.querySelectorAll('[title]');
        
        elementsWithTitles.forEach(function(element) {
            // Prevent default browser tooltip
            const title = element.getAttribute('title');
            if (title) {
                element.setAttribute('data-tooltip', title);
                element.removeAttribute('title');
            }
        });
    }

    /**
     * Handle keyboard navigation
     */
    function initializeKeyboardNavigation() {
        // Escape key to close modals and dropdowns
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                // Close any open modals
                const modals = document.querySelectorAll('.modal-open');
                modals.forEach(modal => modal.classList.remove('modal-open'));
                
                // Close any open dropdowns
                const openDetails = document.querySelectorAll('details[open]');
                openDetails.forEach(details => details.removeAttribute('open'));
                
                // Close drawer on mobile
                const drawerToggle = document.getElementById('main-drawer');
                if (drawerToggle && window.innerWidth < 1024) {
                    drawerToggle.checked = false;
                }
            }
        });
    }

    /**
     * Performance monitoring (development only)
     */
    function initializePerformanceMonitoring() {
        if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
            // Log page load performance
            window.addEventListener('load', function() {
                setTimeout(() => {
                    const perfData = performance.getEntriesByType('navigation')[0];
                    if (perfData) {
                        console.log('Page Performance:', {
                            'DOM Content Loaded': Math.round(perfData.domContentLoadedEventEnd - perfData.fetchStart) + 'ms',
                            'Page Load Complete': Math.round(perfData.loadEventEnd - perfData.fetchStart) + 'ms',
                            'Theme': window.SpydersoftTheme ? window.SpydersoftTheme.get() : 'unknown'
                        });
                    }
                }, 100);
            });
        }
    }

    /**
     * Main initialization function
     */
    function initialize() {
        try {
            enhanceThemeToggle();
            initializeDrawer();
            initializeDropdowns();
            enhanceForms();
            initializeTooltips();
            initializeKeyboardNavigation();
            initializePerformanceMonitoring();

            // Listen for theme changes
            document.addEventListener('themeChanged', function(event) {
                updateThemeElements(event.detail.theme);
            });

            // Initial theme element update
            updateThemeElements(window.SpydersoftTheme.get());

            console.log('Spydersoft Identity - Main JavaScript initialized');
        } catch (error) {
            console.error('Error initializing main JavaScript:', error);
        }
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }

    // Expose utilities globally for other scripts
    window.SpydersoftUI = {
        enhanceThemeToggle: enhanceThemeToggle,
        updateThemeElements: updateThemeElements
    };

})();