// jQuery 4 compatibility shim
// Restores APIs removed in jQuery 4 that are still used by jquery-validation
// and jquery-validation-unobtrusive.
(function ($) {
  if (!$ || typeof $ !== 'function') { return; }

  // Removed in jQuery 4 — use JSON.parse
  if (!$.parseJSON) {
    $.parseJSON = function (data) { return JSON.parse(data); };
  }

  // Removed in jQuery 4 — use typeof fn === "function"
  if (!$.isFunction) {
    $.isFunction = function (obj) { return typeof obj === 'function'; };
  }

  // Removed in jQuery 4 — use String.prototype.trim
  if (!$.trim) {
    $.trim = function (str) { return str == null ? '' : String.prototype.trim.call(str); };
  }
}(window.jQuery));
