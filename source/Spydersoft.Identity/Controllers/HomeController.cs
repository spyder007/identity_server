using System.IO;
using System.Threading.Tasks;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Models;
using Spydersoft.Identity.Models.Home;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class HomeController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    public class HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment hostingEnvironment) : Controller
    {
        /// <summary>
        /// The interaction
        /// </summary>
        private readonly IIdentityServerInteractionService _interaction = interaction;
        /// <summary>
        /// The hosting environment
        /// </summary>
        private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Abouts this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult About()
        {
            var aboutDataFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "about_data.json");
            var model = AboutDataViewModel.LoadFromFile(aboutDataFilePath);

            return View(model);
        }

        /// <summary>
        /// Contacts this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        /// <param name="errorId">The error identifier.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

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