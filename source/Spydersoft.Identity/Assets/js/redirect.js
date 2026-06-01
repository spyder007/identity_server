/**
 * Auto-navigates to the redirect URL captured on the page. Used by the native
 * client "loading" page. External (not inline) to satisfy the CSP.
 */
window.addEventListener('load', function () {
  var el = document.getElementById('redirectUrl');
  if (el) {
    var url = el.getAttribute('data-url');
    if (url) {
      window.location = url;
    }
  }
});
