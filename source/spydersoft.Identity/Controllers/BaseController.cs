using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace spydersoft.Identity.Controllers
{
    public class BaseController : Controller
    {

        public IMapper Mapper { get; }

        public BaseController(IMapper mapper)
        {
            Mapper = mapper;
        }

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