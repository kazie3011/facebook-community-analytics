#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "48e2de24dfbb008d5a1df67c9bec16d0c73c41ae"
// <auto-generated/>
#pragma warning disable 1591
namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using FacebookCommunityAnalytics.Api.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Blazorise;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Blazorise.DataGrid;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Blazorise.Components;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Volo.Abp.BlazoriseUI;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Volo.Abp.BlazoriseUI.Components;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Blazorise.Charts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Blazorise.RichTextEdit;

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using FacebookCommunityAnalytics.Api.Blazor.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using ChartJs.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using BlazorDateRangePicker;

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\_Imports.razor"
using FacebookCommunityAnalytics.Api.Blazor.Shared.Components.Dashboards;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using FacebookCommunityAnalytics.Api.Permissions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using FacebookCommunityAnalytics.Api.Statistics;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using System.Drawing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using FacebookCommunityAnalytics.Api.AffiliateStats;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using FacebookCommunityAnalytics.Api.Core.Const;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using FacebookCommunityAnalytics.Api.Core.Enums;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using ChartJsColor = System.Drawing.Color;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using Color = Blazorise.Color;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using Volo.Abp.AspNetCore.Components.Web.Theming.Layout;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
using Faso.Blazor.SpinKit;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
           [Authorize(ApiPermissions.UserAffiliates.Default)]

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/affsummary")]
    public partial class AffSummary : BlazorComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenComponent<Volo.Abp.AspNetCore.Components.Web.Theming.Layout.PageHeader>(0);
            __builder.AddAttribute(1, "Title", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 18 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                    L["Aff.Summary.Title"]

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(2, "BreadcrumbItems", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.List<Volo.Abp.BlazoriseUI.BreadcrumbItem>>(
#nullable restore
#line 18 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                             BreadcrumbItems

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(3, "Toolbar", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars.PageToolbar>(
#nullable restore
#line 18 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                                       Toolbar

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(4, "\r\n\r\n");
            __builder.OpenComponent<Blazorise.Row>(5);
            __builder.AddAttribute(6, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.Field>(7);
                __builder2.AddAttribute(8, "ColumnSize", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.IFluentColumn>(
#nullable restore
#line 22 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                       ColumnSize.Is6.OnDesktop.IsHalf.OnMobile

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(9, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<Blazorise.Addons>(10);
                    __builder3.AddAttribute(11, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
                        __builder4.OpenComponent<Blazorise.Addon>(12);
                        __builder4.AddAttribute(13, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 24 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                              AddonType.Start

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(14, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<Blazorise.AddonLabel>(15);
                            __builder5.AddAttribute(16, "Class", "bg-primary text-white");
                            __builder5.AddAttribute(17, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder6) => {
#nullable restore
#line 25 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
__builder6.AddContent(18, L["Icon.Date"]);

#line default
#line hidden
#nullable disable
                            }
                            ));
                            __builder5.CloseComponent();
                        }
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddMarkupContent(19, "\r\n            ");
                        __builder4.OpenComponent<Blazorise.Addon>(20);
                        __builder4.AddAttribute(21, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                              AddonType.Body

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(22, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<BlazorDateRangePicker.DateRangePicker>(23);
                            __builder5.AddAttribute(24, "Ranges", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.Dictionary<System.String, BlazorDateRangePicker.DateRange>>(
#nullable restore
#line 29 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                            DateRanges

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(25, "DateFormat", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 30 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                  GlobalConsts.DateFormat

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(26, "StartDate", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.DateTimeOffset?>(
#nullable restore
#line 31 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                               StartDate

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(27, "EndDate", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.DateTimeOffset?>(
#nullable restore
#line 32 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                             EndDate

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(28, "EndDateChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.DateTimeOffset?>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTimeOffset?>(this, 
#nullable restore
#line 33 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                    EndDateChanged

#line default
#line hidden
#nullable disable
                            )));
                            __builder5.AddAttribute(29, "StartDateChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.DateTimeOffset?>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTimeOffset?>(this, 
#nullable restore
#line 34 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                      StartDateChanged

#line default
#line hidden
#nullable disable
                            )));
                            __builder5.AddAttribute(30, "class", "form-control form-control-md");
                            __builder5.AddAttribute(31, "placeholder", 
#nullable restore
#line 36 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                  L["SelectDates"]

#line default
#line hidden
#nullable disable
                            );
                            __builder5.CloseComponent();
                        }
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddMarkupContent(32, "\r\n            ");
                        __builder4.OpenComponent<Blazorise.Addon>(33);
                        __builder4.AddAttribute(34, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 38 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                              AddonType.Body

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(35, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<Blazorise.Select<AffiliateSummaryType>>(36);
                            __builder5.AddAttribute(37, "SelectedValue", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<AffiliateSummaryType>(
#nullable restore
#line 39 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                     AffiliateSummaryType

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(38, "SelectedValueChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<AffiliateSummaryType>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<AffiliateSummaryType>(this, 
#nullable restore
#line 39 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                                                                 UpdateTypeChart

#line default
#line hidden
#nullable disable
                            )));
                            __builder5.AddAttribute(39, "Style", "height: 40px");
                            __builder5.AddAttribute(40, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder6) => {
#nullable restore
#line 40 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                     foreach (var itemValue in Enum.GetValues(typeof(AffiliateSummaryType)))
                    {

#line default
#line hidden
#nullable disable
                                __builder6.OpenComponent<Blazorise.SelectItem<AffiliateSummaryType>>(41);
                                __builder6.AddAttribute(42, "Value", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<AffiliateSummaryType>(
#nullable restore
#line 42 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                                            (AffiliateSummaryType) itemValue

#line default
#line hidden
#nullable disable
                                ));
                                __builder6.AddAttribute(43, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder7) => {
#nullable restore
#line 43 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
__builder7.AddContent(44, L[$"Enum:AffiliateSummaryType:{Convert.ToInt32(itemValue)}"]);

#line default
#line hidden
#nullable disable
                                }
                                ));
                                __builder6.SetKey(
#nullable restore
#line 42 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                          itemValue

#line default
#line hidden
#nullable disable
                                );
                                __builder6.CloseComponent();
#nullable restore
#line 45 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                    }

#line default
#line hidden
#nullable disable
                            }
                            ));
                            __builder5.CloseComponent();
                            __builder5.AddMarkupContent(45, "\r\n                ");
                            __builder5.OpenComponent<Blazorise.Button>(46);
                            __builder5.AddAttribute(47, "Color", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.Color>(
#nullable restore
#line 47 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                               Color.Primary

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(48, "Clicked", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, 
#nullable restore
#line 47 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                       GetData

#line default
#line hidden
#nullable disable
                            )));
                            __builder5.AddAttribute(49, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder6) => {
                                __builder6.OpenComponent<Blazorise.Icon>(50);
                                __builder6.AddAttribute(51, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 48 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                IconName.Search

#line default
#line hidden
#nullable disable
                                ));
                                __builder6.CloseComponent();
                            }
                            ));
                            __builder5.CloseComponent();
                        }
                        ));
                        __builder4.CloseComponent();
                    }
                    ));
                    __builder3.CloseComponent();
                }
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
#nullable restore
#line 57 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
 if (!_showChart)
{

#line default
#line hidden
#nullable disable
            __builder.OpenComponent<Faso.Blazor.SpinKit.SpinKitChasingDots>(52);
            __builder.CloseComponent();
#nullable restore
#line 60 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
}

#line default
#line hidden
#nullable disable
            __builder.OpenComponent<Blazorise.Row>(53);
            __builder.AddAttribute(54, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.Column>(55);
                __builder2.AddAttribute(56, "ColumnSize", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.IFluentColumn>(
#nullable restore
#line 62 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                        ColumnSize.IsFull

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(57, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<ChartJs.Blazor.Chart>(58);
                    __builder3.AddAttribute(59, "Config", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<ChartJs.Blazor.Common.ConfigBase>(
#nullable restore
#line 63 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                      _config

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(60, "Height", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32?>(
#nullable restore
#line 63 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                     120

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(61, "SetupCompletedCallback", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, 
#nullable restore
#line 63 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                                                                   SetupCompletedCallback

#line default
#line hidden
#nullable disable
                    )));
                    __builder3.AddComponentReferenceCapture(62, (__value) => {
#nullable restore
#line 63 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\AffSummary.razor"
                                                     _chart = (ChartJs.Blazor.Chart)__value;

#line default
#line hidden
#nullable disable
                    }
                    );
                    __builder3.CloseComponent();
                }
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime JSRuntime { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IAffiliateStatsAppService AffiliateStatsAppService { get; set; }
    }
}
#pragma warning restore 1591
