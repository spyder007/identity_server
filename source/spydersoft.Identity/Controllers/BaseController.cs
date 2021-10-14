using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Duende.IdentityServer.EntityFramework.DbContexts;
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
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
