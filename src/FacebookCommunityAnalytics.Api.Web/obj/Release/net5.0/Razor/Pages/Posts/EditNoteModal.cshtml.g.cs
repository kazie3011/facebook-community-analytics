#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8db840a751241ef75344a5dd066480589e999542"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Pages_Posts_EditNoteModal), @"mvc.1.0.razor-page", @"/Pages/Posts/EditNoteModal.cshtml")]
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
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
using Microsoft.AspNetCore.Mvc.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
using FacebookCommunityAnalytics.Api.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
using FacebookCommunityAnalytics.Api.Localization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8db840a751241ef75344a5dd066480589e999542", @"/Pages/Posts/EditNoteModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"229e689a848d94625f3fd15edcb2d99cbdb5996f", @"/Pages/_ViewImports.cshtml")]
    #nullable restore
    public class Pages_Posts_EditNoteModal : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("data-ajaxForm", new global::Microsoft.AspNetCore.Html.HtmlString("true"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page", "/Posts/EditNoteModal", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("autocomplete", new global::Microsoft.AspNetCore.Html.HtmlString("off"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalHeaderTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalHeaderTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalBodyTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalBodyTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form.AbpInputTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper;
        private global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalFooterTagHelper __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalFooterTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 8 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8db840a751241ef75344a5dd066480589e9995426258", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-modal", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8db840a751241ef75344a5dd066480589e9995426520", async() => {
                    WriteLiteral("\r\n        ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-modal-header", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8db840a751241ef75344a5dd066480589e9995426799", async() => {
                    }
                    );
                    __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalHeaderTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalHeaderTagHelper>();
                    __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalHeaderTagHelper);
                    BeginWriteTagHelperAttribute();
#nullable restore
#line (14,35)-(14,52) 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
WriteLiteral(L["Update"].Value);

#line default
#line hidden
#nullable disable
                    __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                    __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalHeaderTagHelper.Title = __tagHelperStringValueBuffer;
                    __tagHelperExecutionContext.AddTagHelperAttribute("title", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalHeaderTagHelper.Title, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                    await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                    if (!__tagHelperExecutionContext.Output.IsContentModified)
                    {
                        await __tagHelperExecutionContext.SetOutputContentAsync();
                    }
                    Write(__tagHelperExecutionContext.Output);
                    __tagHelperExecutionContext = __tagHelperScopeManager.End();
                    WriteLiteral("\r\n\r\n        ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-modal-body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8db840a751241ef75344a5dd066480589e9995428722", async() => {
                        WriteLiteral("\r\n            ");
                        __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "8db840a751241ef75344a5dd066480589e9995429018", async() => {
                        }
                        );
                        __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form.AbpInputTagHelper>();
                        __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper);
#nullable restore
#line 17 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.AspFor = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Id);

#line default
#line hidden
#nullable disable
                        __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.AspFor, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                        await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                        if (!__tagHelperExecutionContext.Output.IsContentModified)
                        {
                            await __tagHelperExecutionContext.SetOutputContentAsync();
                        }
                        Write(__tagHelperExecutionContext.Output);
                        __tagHelperExecutionContext = __tagHelperScopeManager.End();
                        WriteLiteral("\r\n\r\n            ");
                        __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "8db840a751241ef75344a5dd066480589e99954210802", async() => {
                        }
                        );
                        __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form.AbpInputTagHelper>();
                        __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper);
#nullable restore
#line 19 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.AspFor = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.PostNoteUpdate);

#line default
#line hidden
#nullable disable
                        __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.AspFor, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                        BeginWriteTagHelperAttribute();
#nullable restore
#line (19,57)-(19,72) 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
WriteLiteral(L["Note"].Value);

#line default
#line hidden
#nullable disable
                        __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                        __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.Label = __tagHelperStringValueBuffer;
                        __tagHelperExecutionContext.AddTagHelperAttribute("label", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Form_AbpInputTagHelper.Label, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                        await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                        if (!__tagHelperExecutionContext.Output.IsContentModified)
                        {
                            await __tagHelperExecutionContext.SetOutputContentAsync();
                        }
                        Write(__tagHelperExecutionContext.Output);
                        __tagHelperExecutionContext = __tagHelperScopeManager.End();
                        WriteLiteral("\r\n        ");
                    }
                    );
                    __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalBodyTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalBodyTagHelper>();
                    __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalBodyTagHelper);
                    await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                    if (!__tagHelperExecutionContext.Output.IsContentModified)
                    {
                        await __tagHelperExecutionContext.SetOutputContentAsync();
                    }
                    Write(__tagHelperExecutionContext.Output);
                    __tagHelperExecutionContext = __tagHelperScopeManager.End();
                    WriteLiteral("\r\n\r\n        ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("abp-modal-footer", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8db840a751241ef75344a5dd066480589e99954214269", async() => {
                    }
                    );
                    __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalFooterTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalFooterTagHelper>();
                    __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalFooterTagHelper);
#nullable restore
#line 22 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalFooterTagHelper.Buttons = (AbpModalButtons.Cancel | AbpModalButtons.Save);

#line default
#line hidden
#nullable disable
                    __tagHelperExecutionContext.AddTagHelperAttribute("buttons", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalFooterTagHelper.Buttons, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                    await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                    if (!__tagHelperExecutionContext.Output.IsContentModified)
                    {
                        await __tagHelperExecutionContext.SetOutputContentAsync();
                    }
                    Write(__tagHelperExecutionContext.Output);
                    __tagHelperExecutionContext = __tagHelperScopeManager.End();
                    WriteLiteral("\r\n    ");
                }
                );
                __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalTagHelper = CreateTagHelper<global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalTagHelper>();
                __tagHelperExecutionContext.Add(__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalTagHelper);
#nullable restore
#line 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Web\Pages\Posts\EditNoteModal.cshtml"
__Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalTagHelper.Size = global::Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal.AbpModalSize.Default;

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("size", __Volo_Abp_AspNetCore_Mvc_UI_Bootstrap_TagHelpers_Modal_AbpModalTagHelper.Size, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Page = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<FacebookCommunityAnalytics.Api.Web.Pages.Posts.EditNoteModalModel> Html { get; private set; } = default!;
        #nullable disable
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<FacebookCommunityAnalytics.Api.Web.Pages.Posts.EditNoteModalModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<FacebookCommunityAnalytics.Api.Web.Pages.Posts.EditNoteModalModel>)PageContext?.ViewData;
        public FacebookCommunityAnalytics.Api.Web.Pages.Posts.EditNoteModalModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591