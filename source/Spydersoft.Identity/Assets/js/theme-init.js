/**
 * Render-blocking theme bootstrap. Loaded synchronously in <head> BEFORE the
 * stylesheet so the correct light/dark theme is set before first paint (no FOUC).
 * Kept as an external file because the strict CSP (default-src 'self') blocks
 * inline scripts.
 */
(function () {
  try {
    var t = localStorage.getItem('spydersoft-theme');
    if (t !== 'light' && t !== 'dark') {
      t = (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) ? 'dark' : 'light';
    }
    document.documentElement.setAttribute('data-theme', t);
  } catch (e) {
    document.documentElement.setAttribute('data-theme', 'light');
  }
})();
