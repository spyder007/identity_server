using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace Spydersoft.Identity.Controllers
{
    public class BaseController(IMapper mapper) : Controller
    {

        public IMapper Mapper { get; } = mapper;

        public IActionResult GetErrorAction(string errorMessage)
        {
            return RedirectToAction(nameof(HomeController.Error), "Home", new { errorId = errorMessage });
        }
        protected IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}