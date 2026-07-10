/**
 * Bootstraps aspnet-client-validation (the jQuery-free replacement for
 * jquery-validation-unobtrusive). Reads the same data-val-* attributes that
 * ASP.NET emits. External file because the CSP forbids inline scripts.
 */
(function () {
  if (window.aspnetValidation) {
    var v = new window.aspnetValidation.ValidationService();
    v.bootstrap();
  }
})();
