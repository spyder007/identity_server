using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Models.Home;

namespace Spydersoft.Identity.Pages
{
    /// <summary>About page listing the technology stack and libraries.</summary>
    public class AboutModel(IWebHostEnvironment hostingEnvironment) : PageModel
    {
        /// <summary>The libraries/technology data loaded from <c>about_data.json</c>.</summary>
        public AboutDataViewModel Data { get; private set; }

        /// <summary>Loads the about data and renders the page.</summary>
        public void OnGet()
        {
            var aboutDataFilePath = Path.Combine(hostingEnvironment.WebRootPath, "about_data.json");
            Data = AboutDataViewModel.LoadFromFile(aboutDataFilePath);
        }
    }
}