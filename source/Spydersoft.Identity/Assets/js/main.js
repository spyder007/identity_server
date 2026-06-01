/**
 * Main JavaScript for Spydersoft Identity.
 * Vanilla (no jQuery). Handles theme toggle, mobile sidebar, dropdowns, and a
 * few page-specific helpers (authenticator QR + copy, disable-2FA toggle).
 */
(function () {
  'use strict';

  var STORAGE_KEY = 'spydersoft-theme';

  /* ---------------------------------------------------------------- Theme -- */
  function currentTheme() {
    var t = document.documentElement.getAttribute('data-theme');
    return t === 'dark' ? 'dark' : 'light';
  }

  function applyTheme(theme) {
    if (theme !== 'light' && theme !== 'dark') {
      theme = 'light';
    }
    document.documentElement.setAttribute('data-theme', theme);
    try {
      localStorage.setItem(STORAGE_KEY, theme);
    } catch (e) { /* ignore */ }

    // Swap any sun/moon icons: show the icon that represents the *active* theme.
    document.querySelectorAll('[data-theme-icon]').forEach(function (el) {
      var want = theme === 'dark' ? 'moon' : 'sun';
      el.classList.toggle('hidden', el.getAttribute('data-theme-icon') !== want);
    });
  }

  window.SpydersoftTheme = {
    get: currentTheme,
    set: applyTheme,
    toggle: function () {
      applyTheme(currentTheme() === 'dark' ? 'light' : 'dark');
    }
  };

  function initTheme() {
    applyTheme(currentTheme());
    document.querySelectorAll('[data-toggle-theme]').forEach(function (btn) {
      btn.addEventListener('click', function (e) {
        e.preventDefault();
        window.SpydersoftTheme.toggle();
      });
    });
  }

  /* -------------------------------------------------------------- Sidebar -- */
  function initSidebar() {
    var sidebar = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebar-overlay');
    var openBtn = document.getElementById('sidebar-open');
    if (!sidebar) {
      return;
    }

    function open() {
      sidebar.classList.remove('-translate-x-full');
      if (overlay) {
        overlay.classList.remove('hidden');
      }
    }

    function close() {
      sidebar.classList.add('-translate-x-full');
      if (overlay) {
        overlay.classList.add('hidden');
      }
    }

    if (openBtn) {
      openBtn.addEventListener('click', function (e) {
        e.preventDefault();
        open();
      });
    }
    if (overlay) {
      overlay.addEventListener('click', close);
    }
    sidebar.querySelectorAll('a[href]').forEach(function (a) {
      a.addEventListener('click', function () {
        if (window.innerWidth < 1024) {
          close();
        }
      });
    });
    window.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') {
        close();
      }
    });
    window.addEventListener('resize', function () {
      if (window.innerWidth >= 1024) {
        close();
      }
    });
  }

  /* ------------------------------------------------------------ Dropdowns -- */
  function closeAllDropdowns(except) {
    document.querySelectorAll('[data-dropdown-menu]').forEach(function (menu) {
      if (menu !== except) {
        menu.classList.add('hidden');
      }
    });
  }

  function initDropdowns() {
    document.querySelectorAll('[data-dropdown-trigger]').forEach(function (trigger) {
      var menu = trigger.parentElement.querySelector('[data-dropdown-menu]');
      if (!menu) {
        return;
      }
      trigger.addEventListener('click', function (e) {
        e.stopPropagation();
        var isHidden = menu.classList.contains('hidden');
        closeAllDropdowns(menu);
        menu.classList.toggle('hidden', !isHidden);
      });
    });

    document.addEventListener('click', function () { closeAllDropdowns(null); });
    window.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') {
        closeAllDropdowns(null);
      }
    });
  }

  /* ------------------------------------------------- Authenticator (QR) ---- */
  function initQrCode() {
    var qrCodeElement = document.getElementById('qrCode');
    var qrCodeData = document.getElementById('qrCodeData');
    if (qrCodeElement && qrCodeData && typeof QRCode !== 'undefined') {
      var url = qrCodeData.getAttribute('data-url');
      if (url) {
        qrCodeElement.innerHTML = '';
        new QRCode(qrCodeElement, {
          text: url,
          width: 180,
          height: 180,
          colorDark: '#000000',
          colorLight: '#ffffff'
        });
      }
    }

    // Auto-focus the verification code input when present.
    var codeInput = document.querySelector('input[name="Code"], input[name="Input.Code"]');
    if (codeInput) {
      codeInput.focus();
    }
  }

  /* --------------------------------------------------------------- Copy ---- */
  function copyValue(text, button) {
    if (!navigator.clipboard) {
      return;
    }
    navigator.clipboard.writeText(text).then(function () {
      if (button) {
        var original = button.innerHTML;
        button.innerHTML = '<i class="fas fa-check"></i>';
        setTimeout(function () { button.innerHTML = original; }, 1500);
      }
    });
  }

  function initCopyButtons() {
    document.querySelectorAll('[data-copy-target]').forEach(function (button) {
      button.addEventListener('click', function (e) {
        e.preventDefault();
        var target = document.getElementById(button.getAttribute('data-copy-target'));
        if (target) {
          copyValue(target.value || target.textContent || '', button);
        }
      });
    });
    document.querySelectorAll('[data-copy-text]').forEach(function (button) {
      button.addEventListener('click', function (e) {
        e.preventDefault();
        copyValue(button.getAttribute('data-copy-text') || '', button);
      });
    });
  }

  /* --------------------------------------------------------- Disable 2FA --- */
  function initDisable2fa() {
    var confirmCheckbox = document.getElementById('confirmDisable');
    var disableButton = document.getElementById('disableButton');
    if (confirmCheckbox && disableButton) {
      disableButton.disabled = !confirmCheckbox.checked;
      confirmCheckbox.addEventListener('change', function () {
        disableButton.disabled = !this.checked;
      });
    }
  }

  /* ----------------------------------------------------------- Bootstrap --- */
  function initialize() {
    initTheme();
    initSidebar();
    initDropdowns();
    initQrCode();
    initCopyButtons();
    initDisable2fa();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initialize);
  } else {
    initialize();
  }
})();
