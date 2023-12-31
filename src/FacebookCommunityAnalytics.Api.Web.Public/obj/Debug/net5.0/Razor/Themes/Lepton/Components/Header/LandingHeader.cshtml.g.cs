#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ae94e179588980a5872d6cbbf1b1a95ff1b06f1d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Themes_Lepton_Components_Header_LandingHeader), @"mvc.1.0.view", @"/Themes/Lepton/Components/Header/LandingHeader.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Header.ToolBar;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Header.Brand;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.MainMenu;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ae94e179588980a5872d6cbbf1b1a95ff1b06f1d", @"/Themes/Lepton/Components/Header/LandingHeader.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8e98020b60f64049f46ede4a2d8bbb18719a2b4b", @"/Themes/_ViewImports.cshtml")]
    #nullable restore
    public class Themes_Lepton_Components_Header_LandingHeader : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<header class=\"landing-header\">\r\n    <div class=\"lp-content\">\r\n        ");
#nullable restore
#line (7,11)-(7,66) 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
Write(await Component.InvokeAsync<HeaderBrandViewComponent>());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"

        <nav class=""navbar navbar-expand-xl navbar-dark d-lg-none nav-mobile"">
            <button class=""navbar-toggler border-0"" type=""button"" data-toggle=""collapse"" data-target=""#navbarToolbar"" aria-controls=""navbarText"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                <i class=""fa fa-user""></i>
            </button>
            <button class=""navbar-toggler border-0"" type=""button"" data-toggle=""collapse"" data-target=""#navbarSidebar"" aria-controls=""navbarText"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                <i class=""fa fa-align-justify""></i>
            </button>
        </nav>

        <nav class=""navbar navbar-expand-lg user-nav-mobile"">
            <div class=""collapse navbar-collapse d-lg-block toolbar-nav-wrapper"" id=""navbarToolbar"">
                <ul class=""navbar-nav toolbar-nav ml-auto"">
                    ");
#nullable restore
#line (21,23)-(21,80) 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
Write(await Component.InvokeAsync<HeaderToolBarViewComponent>());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </ul>\r\n            </div>\r\n        </nav>\r\n\r\n        ");
#nullable restore
#line (26,11)-(26,63) 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Themes\Lepton\Components\Header\LandingHeader.cshtml"
Write(await Component.InvokeAsync<MainMenuViewComponent>());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n</header>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
