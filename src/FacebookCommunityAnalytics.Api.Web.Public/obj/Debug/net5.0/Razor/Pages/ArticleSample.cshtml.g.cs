#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fa045ebf3e19b9f81328c02e7843c426881d87c0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Pages_ArticleSample), @"mvc.1.0.razor-page", @"/Pages/ArticleSample.cshtml")]
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
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
using Microsoft.AspNetCore.Mvc.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
using FacebookCommunityAnalytics.Api.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
using Volo.Abp.GlobalFeatures;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
using Volo.CmsKit.GlobalFeatures;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
using Volo.CmsKit.Public.Web.Pages.CmsKit.Shared.Components.Commenting;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute("RouteTemplate", "/article-sample")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fa045ebf3e19b9f81328c02e7843c426881d87c0", @"/Pages/ArticleSample.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8e98020b60f64049f46ede4a2d8bbb18719a2b4b", @"/Pages/_ViewImports.cshtml")]
    #nullable restore
    public class Pages_ArticleSample : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    #nullable disable
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardHeaderTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardHeaderTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardBodyTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardBodyTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardFooterTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardFooterTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-card", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa045ebf3e19b9f81328c02e7843c426881d87c04880", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-card-header", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa045ebf3e19b9f81328c02e7843c426881d87c05146", async() => {
                    WriteLiteral("\r\n        <h2>\r\n            Article Sample\r\n        </h2>\r\n    ");
                }
                );
                __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardHeaderTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardHeaderTagHelper>();
                __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardHeaderTagHelper);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-card-body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa045ebf3e19b9f81328c02e7843c426881d87c06310", async() => {
                    WriteLiteral(@"
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed venenatis dui dictum, tempor elit vel, commodo quam. Fusce eu mi ac leo tristique finibus a eu leo. Aenean posuere tortor enim, a porta enim bibendum ut. Nam ante augue, suscipit quis felis sit amet, posuere interdum enim. Mauris vulputate pellentesque orci, at tincidunt eros ultricies a. Ut vulputate nulla in metus convallis, eget venenatis dolor finibus. Morbi elementum lacus in iaculis malesuada. Nam vestibulum, enim in lobortis mattis, elit diam commodo nulla, eu laoreet erat massa sed velit. Donec feugiat justo dui, ut consequat augue aliquet quis. Integer ut tincidunt felis. Nulla dignissim, magna ut lobortis molestie, orci augue venenatis leo, eget lacinia metus felis vitae velit. Morbi eu ipsum ullamcorper, mollis est ut, venenatis purus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae;
        </p>

        <p>
            Donec congue, ex non ullamcorper consectetur, met");
                    WriteLiteral(@"us arcu varius sapien, in pellentesque sem orci in purus. Pellentesque ullamcorper pretium eros, eget tempor nibh finibus id. Pellentesque euismod, nisi eu tincidunt malesuada, lacus eros auctor purus, nec suscipit orci arcu non enim. Mauris quis leo felis. Suspendisse sit amet sapien at turpis tincidunt malesuada. Nunc porttitor magna vitae dictum feugiat. Nullam facilisis facilisis erat quis sodales. Ut at est quis ex tincidunt malesuada. Maecenas at nunc urna. Sed malesuada sollicitudin massa, sed interdum lectus varius vitae. Ut risus nisi, imperdiet eget arcu non, venenatis consectetur erat. Praesent vulputate, dolor vitae interdum luctus, dolor mauris congue quam, eget pulvinar massa velit ultrices purus. Fusce sodales massa quis sem iaculis ullamcorper. Integer dolor neque, imperdiet sed nunc non, scelerisque luctus urna. Nulla mattis pellentesque eros, id aliquet tellus vehicula sit amet.
        </p>

        <p>
            Aenean iaculis, lacus ut ullamcorper faucibus, tortor magna posuere ligu");
                    WriteLiteral(@"la, vitae sollicitudin erat orci eu diam. Duis lobortis leo sit amet dignissim tincidunt. Morbi lacinia egestas egestas. Maecenas facilisis nisi at dui laoreet, ac euismod justo tempor. In efficitur, nisi vitae aliquam commodo, odio nisi dignissim lorem, eu commodo lectus purus sit amet leo. Integer odio ipsum, imperdiet et diam a, molestie accumsan arcu. Phasellus mattis nunc nisl, et accumsan orci condimentum ac. Morbi placerat nisl ut tincidunt interdum. Praesent pulvinar pretium mollis. Aenean id ligula mollis, bibendum augue non, pretium metus. Donec rutrum, purus vitae tincidunt hendrerit, diam purus sagittis diam, in congue ante lorem ut nisl. Nulla mattis, risus at tempus tristique, dolor ipsum ullamcorper est, nec egestas arcu ex quis sapien. Vivamus nisi risus, suscipit nec nisl eget, suscipit lacinia turpis. Ut tempus gravida elit, quis porta turpis fermentum in. Sed volutpat nulla vitae accumsan sagittis.
        </p>

        <p>
            Proin consectetur velit sem, ut porttitor erat grav");
                    WriteLiteral(@"ida a. Praesent a erat vel tellus molestie vulputate. Fusce rhoncus dignissim aliquet. Ut auctor eros malesuada, semper lorem eu, laoreet nisl. Aliquam fermentum sed tellus sed faucibus. Nam eu mauris vel nisi scelerisque suscipit ut id enim. Cras condimentum ligula at sem semper, in varius ligula commodo. Fusce lobortis lectus erat, ut dignissim massa lobortis in. Curabitur ac sapien mi. Aliquam nec enim lacus.
        </p>

        <p>
            Vivamus in augue in nibh laoreet congue. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Nunc vitae nisi in orci dictum posuere quis ut sapien. Aliquam quam nulla, aliquam in elit sit amet, semper cursus purus. Phasellus a tortor non est posuere maximus ut sed dui. Phasellus vel lectus velit. Aliquam vel posuere nibh, vehicula blandit quam.
        </p>
    ");
                }
                );
                __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardBodyTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardBodyTagHelper>();
                __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardBodyTagHelper);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 37 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
     if (GlobalFeatureManager.Instance.IsEnabled<CommentsFeature>())
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-card-footer", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa045ebf3e19b9f81328c02e7843c426881d87c011791", async() => {
                    WriteLiteral("\r\n            ");
#nullable restore
#line (40,14)-(41,88) 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
Write(await Component.InvokeAsync(typeof(CommentingViewComponent),
                new { entityType = "SampleArticle", entityId = Guid.Empty.ToString() }));

#line default
#line hidden
#nullable disable
                    WriteLiteral("\r\n        ");
                }
                );
                __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardFooterTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardFooterTagHelper>();
                __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardFooterTagHelper);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 43 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web.Public\Pages\ArticleSample.cshtml"
    }

#line default
#line hidden
#nullable disable
            }
            );
            __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card.AbpCardTagHelper>();
            __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Card_AbpCardTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IHtmlLocalizer<ApiResource> L { get; private set; } = default!;
        #nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<FacebookCommunityAnalytics.Api.Web.Public.Pages.ArticleSampleModel> Html { get; private set; } = default!;
        #nullable disable
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<FacebookCommunityAnalytics.Api.Web.Public.Pages.ArticleSampleModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<FacebookCommunityAnalytics.Api.Web.Public.Pages.ArticleSampleModel>)PageContext?.ViewData;
        public FacebookCommunityAnalytics.Api.Web.Public.Pages.ArticleSampleModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591