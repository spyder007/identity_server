using System.IO;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Home;

namespace spydersoft.Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IIdentityServerInteractionService interaction, IHostingEnvironment hostingEnvironment)
        {
            _interaction = interaction;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var aboutDataFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "about_data.json");
            AboutDataViewModel model = AboutDataViewModel.LoadFromFile(aboutDataFilePath);

            return View(model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }
            else
            {
                vm.Error = new ErrorMessage()
                {
                    Error = errorId
                };
            }

            return View("Error", vm);
        }
    }
}