/**
 * Main JavaScript for Spydersoft Identity Server
 * Handles theme management and general UI functionality
 */

(function() {
    'use strict';

    /**
     * Immediate theme initialization to prevent FOUC
     * This must run as early as possible
     */
    function initializeThemeImmediate() {
        try {
            const savedTheme = localStorage.getItem('spydersoft-theme') || 'identity';
            document.documentElement.setAttribute('data-theme', savedTheme);
        } catch (e) {
            // Fallback if localStorage is not available
            document.documentElement.setAttribute('data-theme', 'identity');
        }
    }

    // Run theme initialization immediately if document is still loading
    if (document.readyState === 'loading') {
        initializeThemeImmediate();
    }

    /**
     * Enhanced theme switching functionality
     * Moved from inline script in _Layout.cshtml
     */
    window.SpydersoftTheme = (function() {
        'use strict';
        
        const STORAGE_KEY = 'spydersoft-theme';
        const DEFAULT_THEME = 'identity';
        const DARK_THEME = 'dark';
        
        function isStorageAvailable() {
            try {
                const test = '__storage_test__';
                localStorage.setItem(test, test);
                localStorage.removeItem(test);
                return true;
            } catch (e) {
                return false;
            }
        }
        
        function getTheme() {
            if (isStorageAvailable()) {
                return localStorage.getItem(STORAGE_KEY) || DEFAULT_THEME;
            }
            // Fallback to checking the current data-theme attribute
            return document.documentElement.getAttribute('data-theme') || DEFAULT_THEME;
        }
        
        function setTheme(theme) {
            try {
                // Validate theme
                if (theme !== DEFAULT_THEME && theme !== DARK_THEME) {
                    theme = DEFAULT_THEME;
                }
                
                document.documentElement.setAttribute('data-theme', theme);
                
                if (isStorageAvailable()) {
                    localStorage.setItem(STORAGE_KEY, theme);
                }
                
                // Dispatch custom event for other components
                if (typeof CustomEvent !== 'undefined') {
                    const event = new CustomEvent('themeChanged', { 
                        detail: { theme: theme } 
                    });
                    document.dispatchEvent(event);
                }
                
                return true;
            } catch (e) {
                console.warn('Failed to set theme:', e);
                return false;
            }
        }
        
        function toggleTheme() {
            const currentTheme = getTheme();
            const newTheme = currentTheme === DEFAULT_THEME ? DARK_THEME : DEFAULT_THEME;
            return setTheme(newTheme);
        }
        
        function initializeTheme() {
            const savedTheme = getTheme();
            setTheme(savedTheme);
        }
        
        // Public API
        return {
            get: getTheme,
            set: setTheme,
            toggle: toggleTheme,
            init: initializeTheme,
            isStorageAvailable: isStorageAvailable
        };
    })();

    // Global function for backward compatibility
    window.toggleTheme = function() {
        return window.SpydersoftTheme.toggle();
    };

    /**
     * Initialize theme system
     * Moved from inline script in _Layout.cshtml
     */
    function initializeThemeSystem() {
        try {
            window.SpydersoftTheme.init();
            
            // Add click listeners to any theme toggle buttons
            const themeToggleButtons = document.querySelectorAll('[data-toggle-theme], .theme-toggle');
            themeToggleButtons.forEach(function(button) {
                button.addEventListener('click', function(e) {
                    e.preventDefault();
                    window.SpydersoftTheme.toggle();
                });
            });
            
            // Debug info for development
            if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
                console.log('Spydersoft Theme initialized:', {
                    currentTheme: window.SpydersoftTheme.get(),
                    storageAvailable: window.SpydersoftTheme.isStorageAvailable()
                });
            }
        } catch (e) {
            console.error('Failed to initialize theme system:', e);
        }
    }

    /**
     * QR Code generation for authenticator setup
     * Moved from Views/Manage/EnableAuthenticator.cshtml
     */
    function initializeQRCodeGeneration() {
        const qrCodeElement = document.getElementById('qrCode');
        const qrCodeData = document.getElementById('qrCodeData');
        
        if (qrCodeElement && qrCodeData && typeof QRCode !== 'undefined') {
            const url = qrCodeData.getAttribute('data-url');
            if (url) {
                qrCodeElement.innerHTML = '';
                new QRCode(qrCodeElement, {
                    text: url,
                    width: 200,
                    height: 200,
                    colorDark: '#000000',
                    colorLight: '#ffffff'
                });
            }
        }
    }

    /**
     * Copy to clipboard functionality
     * Moved from Views/Manage/EnableAuthenticator.cshtml
     */
    window.copyToClipboard = function(elementId) {
        const element = document.getElementById(elementId);
        if (!element) return;
        
        element.select();
        element.setSelectionRange(0, 99999);
        navigator.clipboard.writeText(element.value).then(function() {
            // Show temporary success message
            const btn = element.nextElementSibling;
            if (btn) {
                const originalIcon = btn.innerHTML;
                btn.innerHTML = '<i class="fas fa-check text-success"></i>';
                setTimeout(() => {
                    btn.innerHTML = originalIcon;
                }, 2000);
            }
        });
    };

    /**
     * Initialize copy buttons with data attributes
     */
    function initializeCopyButtons() {
        const copyButtons = document.querySelectorAll('[data-copy-target]');
        copyButtons.forEach(function(button) {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                const targetId = this.getAttribute('data-copy-target');
                window.copyToClipboard(targetId);
            });
        });
    }

    /**
     * User management functionality
     * Moved from Views/Users/Index.cshtml
     */
    function initializeUserSearch() {
        const searchInput = document.getElementById('userSearch');
        if (searchInput) {
            searchInput.addEventListener('input', function() {
                const searchTerm = this.value.toLowerCase();
                const rows = document.querySelectorAll('tbody tr');
                
                rows.forEach(row => {
                    if (row.cells && row.cells.length >= 2) {
                        const username = row.cells[0].textContent.toLowerCase();
                        const email = row.cells[1].textContent.toLowerCase();
                        
                        if (username.includes(searchTerm) || email.includes(searchTerm)) {
                            row.style.display = '';
                        } else {
                            row.style.display = 'none';
                        }
                    }
                });
            });
        }
    }

    /**
     * Initialize delete buttons with data attributes
     */
    function initializeDeleteButtons() {
        const deleteButtons = document.querySelectorAll('[data-delete-id]');
        deleteButtons.forEach(function(button) {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                const itemId = this.getAttribute('data-delete-id');
                const itemName = this.getAttribute('data-delete-name');
                window.confirmDelete(itemId, itemName);
            });
        });
    }

    /**
     * Confirmation dialogs for delete operations
     * Moved from various views
     */
    window.confirmDelete = function(itemId, itemName) {
        const modal = document.getElementById('deleteModal');
        const nameElement = document.getElementById('deleteUserName') || document.getElementById('deleteRoleName');
        const confirmBtn = document.getElementById('deleteConfirmBtn');
        
        if (modal && nameElement && confirmBtn) {
            nameElement.textContent = itemName;
            
            // Determine the correct URL based on current page
            let deleteUrl = '';
            if (window.location.pathname.includes('/Users/')) {
                deleteUrl = window.SpydersoftRoutes?.userDelete?.replace('{id}', itemId) || '/Users/Delete/' + itemId;
            } else if (window.location.pathname.includes('/UserRoles/')) {
                deleteUrl = window.SpydersoftRoutes?.roleDelete?.replace('{id}', itemId) || '/UserRoles/Delete?id=' + itemId;
            } else if (window.location.pathname.includes('/Scopes/')) {
                deleteUrl = window.SpydersoftRoutes?.scopeDelete?.replace('{id}', itemId) || '/Scopes/Delete/' + itemId;
            }
            
            confirmBtn.href = deleteUrl;
            modal.showModal();
        }
    };

    /**
     * Scope filtering functionality
     * Moved from Views/Scopes/Index.cshtml
     */
    window.filterScopes = function(filterType) {
        const rows = document.querySelectorAll('tbody tr[data-scope]');
        
        rows.forEach(row => {
            const scope = JSON.parse(row.getAttribute('data-scope'));
            let show = true;
            
            switch (filterType) {
                case 'enabled':
                    show = scope.enabled === true;
                    break;
                case 'disabled':
                    show = scope.enabled === false;
                    break;
                case 'emphasized':
                    show = scope.emphasize === true;
                    break;
                case 'required':
                    show = scope.required === true;
                    break;
                case 'all':
                default:
                    show = true;
                    break;
            }
            
            row.style.display = show ? '' : 'none';
        });
    };

    /**
     * Enhanced confirm with return value
     * For simple confirmations that need return values
     */
    window.confirmAction = function(message) {
        return confirm(message);
    };

    /**
     * Authenticator setup enhancements
     * Moved from Views/Manage/EnableAuthenticator.cshtml
     */
    function initializeAuthenticatorSetup() {
        // Auto-focus on code input when step 3 is opened
        const codeInput = document.querySelector('input[name="Code"]');
        if (codeInput) {
            const accordionInputs = document.querySelectorAll('input[name="setup-accordion"]');
            if (accordionInputs.length >= 3) {
                const step3Radio = accordionInputs[2];
                step3Radio.addEventListener('change', function() {
                    if (this.checked) {
                        setTimeout(() => codeInput.focus(), 100);
                    }
                });
            }
        }
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
     * Initialize manage section navigation
     */
    function initializeManageNavigation() {
        // Enhanced tab navigation for manage section
        const manageTabs = document.querySelectorAll('.tabs .tab');
        
        manageTabs.forEach(function(tab) {
            // Add hover effects and focus management
            tab.addEventListener('mouseenter', function() {
                if (!this.classList.contains('tab-current')) {
                    this.style.backgroundColor = 'rgb(219 234 254)'; // blue-100
                }
            });
            
            tab.addEventListener('mouseleave', function() {
                if (!this.classList.contains('tab-current')) {
                    this.style.backgroundColor = '';
                }
            });
            
            // Add keyboard navigation
            tab.addEventListener('keydown', function(event) {
                if (event.key === 'Enter' || event.key === ' ') {
                    event.preventDefault();
                    this.click();
                }
            });
        });

        // Add visual feedback when navigating between manage pages
        const manageLinks = document.querySelectorAll('.tabs .tab[href]');
        manageLinks.forEach(function(link) {
            link.addEventListener('click', function(event) {
                // Add a subtle loading effect
                const icon = this.querySelector('i');
                if (icon) {
                    icon.classList.add('fa-spin');
                    setTimeout(() => {
                        icon.classList.remove('fa-spin');
                    }, 500);
                }
            });
        });
    }

    /**
     * Initialize account management enhancements
     */
    function initializeAccountManagement() {
        // Enhanced form validation feedback
        const inputs = document.querySelectorAll('.form-control, .input');
        inputs.forEach(function(input) {
            // Add real-time validation feedback
            input.addEventListener('blur', function() {
                const validationSpan = this.parentNode.querySelector('.text-danger, .text-error');
                if (validationSpan && validationSpan.textContent.trim()) {
                    this.classList.add('input-error');
                } else {
                    this.classList.remove('input-error');
                }
            });
            
            input.addEventListener('input', function() {
                // Remove error state when user starts typing
                this.classList.remove('input-error');
            });
        });

        // Enhanced password visibility toggle
        const passwordInputs = document.querySelectorAll('input[type="password"]');
        passwordInputs.forEach(function(input) {
            // Add password visibility toggle if it doesn't exist
            if (!input.parentNode.querySelector('.password-toggle')) {
                const wrapper = document.createElement('div');
                wrapper.className = 'relative';
                input.parentNode.insertBefore(wrapper, input);
                wrapper.appendChild(input);
                
                const toggleButton = document.createElement('button');
                toggleButton.type = 'button';
                toggleButton.className = 'password-toggle absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-gray-700';
                toggleButton.innerHTML = '<i class="fas fa-eye"></i>';
                toggleButton.setAttribute('aria-label', 'Toggle password visibility');
                
                toggleButton.addEventListener('click', function() {
                    const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
                    input.setAttribute('type', type);
                    
                    const icon = this.querySelector('i');
                    if (type === 'password') {
                        icon.className = 'fas fa-eye';
                        this.setAttribute('aria-label', 'Show password');
                    } else {
                        icon.className = 'fas fa-eye-slash';
                        this.setAttribute('aria-label', 'Hide password');
                    }
                });
                
                wrapper.appendChild(toggleButton);
            }
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
            // Initialize theme system first (moved from inline script)
            initializeThemeSystem();
            
            enhanceThemeToggle();
            initializeDrawer();
            initializeDropdowns();
            enhanceForms();
            initializeManageNavigation();
            initializeAccountManagement();
            initializeTooltips();
            initializeKeyboardNavigation();
            initializePerformanceMonitoring();
            
            // Page-specific initializations
            initializeQRCodeGeneration();
            initializeUserSearch();
            initializeAuthenticatorSetup();
            initializeCopyButtons();
            initializeDeleteButtons();
            initializeConfirmButtons();

            // Listen for theme changes
            document.addEventListener('themeChanged', function(event) {
                updateThemeElements(event.detail.theme);
            });

            // Initial theme element update
            if (window.SpydersoftTheme) {
                updateThemeElements(window.SpydersoftTheme.get());
            }

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
        updateThemeElements: updateThemeElements,
        initializeManageNavigation: initializeManageNavigation,
        initializeAccountManagement: initializeAccountManagement,
        initializeThemeSystem: initializeThemeSystem,
        initializeThemeImmediate: initializeThemeImmediate,
        initializeQRCodeGeneration: initializeQRCodeGeneration,
        initializeUserSearch: initializeUserSearch,
        initializeAuthenticatorSetup: initializeAuthenticatorSetup,
        initializeCopyButtons: initializeCopyButtons,
        initializeDeleteButtons: initializeDeleteButtons,
        initializeConfirmButtons: initializeConfirmButtons
    };

})();