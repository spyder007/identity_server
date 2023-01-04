using System.IO;
using System.Threading.Tasks;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Home;

namespace spydersoft.Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment hostingEnvironment)
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
            var model = AboutDataViewModel.LoadFromFile(aboutDataFilePath);

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
            ErrorMessage message = await _interaction.GetErrorContextAsync(errorId);
            vm.Error = message ?? new ErrorMessage()
            {
                Error = errorId
            };

            return View("Error", vm);
        }
    }
}