using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spydersoft.Identity.Models.Home
{
    /// <summary>
    /// Class AboutDataViewModel.
    /// </summary>
    public class AboutDataViewModel
    {
        /// <summary>
        /// Gets or sets the UI libraries.
        /// </summary>
        /// <value>The UI libraries.</value>
        [JsonPropertyName("uilibraries")]
        public List<Library> UiLibraries { get; set; }

        /// <summary>
        /// Gets or sets the net libraries.
        /// </summary>
        /// <value>The net libraries.</value>
        [JsonPropertyName("netlibraries")]
        public List<Library> NetLibraries { get; set; }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>AboutDataViewModel.</returns>
        public static AboutDataViewModel LoadFromFile(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AboutDataViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }


    /// <summary>
    /// Class Library.
    /// </summary>
    public class Library
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }
}