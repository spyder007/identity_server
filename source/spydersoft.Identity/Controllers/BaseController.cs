using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace spydersoft.Identity.Controllers
{
    public class BaseController : Controller
    {
        public MapperConfiguration AutoMapperConfiguration { get; }

        public IMapper Mapper => AutoMapperConfiguration.CreateMapper();

        public BaseController(MapperConfiguration mapperConfig)
        {
            AutoMapperConfiguration = mapperConfig;
        }

        public IActionResult GetErrorAction(string errorMessage)
        {
            return RedirectToAction(nameof(HomeController.Error), "Home", new { errorId = errorMessage });
        }
    }
}
