using System.Linq;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace spydersoft.Identity.TagHelpers
{
    [HtmlTargetElement(Attributes = ValidationForAttributeName + "," + ValidationErrorClassName)]
    public class ValidationClassTagHelper : TagHelper
    {
        private const string ValidationForAttributeName = "pf-validation-for";
        private const string ValidationErrorClassName = "pf-validationerror-class";

        [HtmlAttributeName(ValidationForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ValidationErrorClassName)]
        public string ValidationErrorClass { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For == null)
            {
                return;
            }

            if (!ViewContext.ViewData.ModelState.TryGetValue(For.Name, out ModelStateEntry entry) || !entry.Errors.Any())
            {
                return;
            }

            output.AddClass(ValidationErrorClass, HtmlEncoder.Default);
        }
    }
}