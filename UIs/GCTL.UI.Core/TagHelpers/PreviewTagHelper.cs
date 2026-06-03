using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCTL.UI.Core.TagHelpers
{
    public class PreviewTagHelper : TagHelper
    {
        // Can be passed via <email mail-to="..." />. 
        // PascalCase gets translated into kebab-case.
        public string MailTo { get; set; }

        //public override void Process(TagHelperContext context, TagHelperOutput output)
        //{
        //    output.TagName = "a";    // Replaces <email> with <a> tag
        //    //<object type="application/pdf" width="100%" height="600" data="@Url.Content("~/PDFViewer/Sample.pdf")" internalinstanceid="7" class="skrollable skrollable-after"></object>
        //    var address = MailTo + "@" + EmailDomain;
        //    output.Attributes.SetAttribute("href", "mailto:" + address);
        //    output.Content.SetContent(address);
        //}
    }
}
