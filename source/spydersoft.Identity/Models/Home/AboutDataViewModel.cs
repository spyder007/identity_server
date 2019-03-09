using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace spydersoft.Identity.Models.Home
{
    public class AboutDataViewModel
    { 
        [JsonProperty("uilibraries")]
        public List<Library> UiLibraries { get; set; }

        [JsonProperty("netlibraries")]
        public List<Library> NetLibraries { get; set; }

        public static AboutDataViewModel LoadFromFile(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AboutDataViewModel>(json);
        }
    }

    
    public class Library
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
