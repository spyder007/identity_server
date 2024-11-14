using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Constants;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class BaseController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    public class BaseController(IMapper mapper) : Controller
    {

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        public IMapper Mapper { get; } = mapper;

        /// <summary>
        /// Gets the error action.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>IActionResult.</returns>
        public IActionResult GetErrorAction(string errorMessage)
        {
            return RedirectToAction(nameof(HomeController.Error), "Home", new { errorId = ModelState.IsValid ? errorMessage : Messages.InvalidRequest });
        }
        /// <summary>
        /// Redirects to local.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>IActionResult.</returns>
        protected IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}