#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "837f090e9389f88dd7645d7e7be73e1bafef285b"
// <auto-generated/>
#pragma warning disable 1591
namespace FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
#line 1 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using FacebookCommunityAnalytics.Api.Permissions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using Microsoft.AspNetCore.Components;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using FacebookCommunityAnalytics.Api.Core.Const;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using FacebookCommunityAnalytics.Api.Core.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using FacebookCommunityAnalytics.Api.PartnerModule;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using Volo.Abp.AspNetCore.Components.Web.Theming.Layout;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using Faso.Blazor.SpinKit;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
           [Authorize(ApiPermissions.PartnerModule.Default)]

#line default
#line hidden
#nullable disable
    public partial class HomePagePartner : BlazorComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenComponent<Volo.Abp.AspNetCore.Components.Web.Theming.Layout.PageHeader>(0);
            __builder.AddAttribute(1, "Title", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                    L["Partner.Dashboard.Title"]

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(2, "BreadcrumbItems", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.List<Volo.Abp.BlazoriseUI.BreadcrumbItem>>(
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                                   BreadcrumbItems

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(3, "Toolbar", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars.PageToolbar>(
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                                                             Toolbar

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(4, "\r\n\r\n");
            __builder.OpenComponent<Blazorise.Card>(5);
            __builder.AddAttribute(6, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.CardBody>(7);
                __builder2.AddAttribute(8, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<Blazorise.Row>(9);
                    __builder3.AddAttribute(10, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
                        __builder4.OpenComponent<Blazorise.Column>(11);
                        __builder4.AddAttribute(12, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<Blazorise.Addons>(13);
                            __builder5.AddAttribute(14, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder6) => {
                                __builder6.OpenComponent<Blazorise.Addon>(15);
                                __builder6.AddAttribute(16, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 23 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                      AddonType.Start

#line default
#line hidden
#nullable disable
                                ));
                                __builder6.AddAttribute(17, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder7) => {
                                    __builder7.OpenComponent<Blazorise.AddonLabel>(18);
                                    __builder7.AddAttribute(19, "Class", "bg-primary text-white");
                                    __builder7.AddAttribute(20, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder8) => {
#nullable restore
#line 24 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
__builder8.AddContent(21, L["Icon.Date"]);

#line default
#line hidden
#nullable disable
                                    }
                                    ));
                                    __builder7.CloseComponent();
                                }
                                ));
                                __builder6.CloseComponent();
                                __builder6.AddMarkupContent(22, "\r\n                    ");
                                __builder6.OpenComponent<Blazorise.Addon>(23);
                                __builder6.AddAttribute(24, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 26 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                      AddonType.Body

#line default
#line hidden
#nullable disable
                                ));
                                __builder6.AddAttribute(25, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder7) => {
                                    __builder7.OpenComponent<BlazorDateRangePicker.DateRangePicker>(26);
                                    __builder7.AddAttribute(27, "TimePicker", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean?>(
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                     true

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(28, "TimePicker24Hour", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean?>(
#nullable restore
#line 28 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                           true

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(29, "Ranges", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.Dictionary<System.String, BlazorDateRangePicker.DateRange>>(
#nullable restore
#line 29 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                 _dateRanges

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(30, "DateFormat", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 30 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                      GlobalConsts.DateFormat

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(31, "class", "form-control form-control-md");
                                    __builder7.AddAttribute(32, "placeholder", 
#nullable restore
#line 34 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                       L["SelectDates"]

#line default
#line hidden
#nullable disable
                                    );
                                    __builder7.AddAttribute(33, "OnClosed", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, 
#nullable restore
#line 34 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                                                   ReloadChartData

#line default
#line hidden
#nullable disable
                                    )));
                                    __builder7.AddAttribute(34, "StartDate", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.DateTimeOffset?>(
#nullable restore
#line 31 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                          StartDate

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(35, "StartDateChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.DateTimeOffset?>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTimeOffset?>(this, global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => StartDate = __value, StartDate))));
                                    __builder7.AddAttribute(36, "EndDate", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.DateTimeOffset?>(
#nullable restore
#line 32 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                                        EndDate

#line default
#line hidden
#nullable disable
                                    ));
                                    __builder7.AddAttribute(37, "EndDateChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.DateTimeOffset?>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTimeOffset?>(this, global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => EndDate = __value, EndDate))));
                                    __builder7.CloseComponent();
                                }
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
                    __builder3.AddMarkupContent(38, "\r\n        ");
                    __builder3.OpenComponent<Blazorise.Row>(39);
                    __builder3.AddAttribute(40, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
                        __builder4.OpenComponent<Blazorise.Column>(41);
                        __builder4.AddAttribute(42, "ColumnSize", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.IFluentColumn>(
#nullable restore
#line 40 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                ColumnSize.IsHalf.OnDesktop.IsFull.OnTablet.IsFull.OnMobile

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(43, "class", "chart-js-custom");
                        __builder4.AddAttribute(44, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<ChartJs.Blazor.Chart>(45);
                            __builder5.AddAttribute(46, "Config", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<ChartJs.Blazor.Common.ConfigBase>(
#nullable restore
#line 41 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                              _piePartnerCampaignChartConfig

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.CloseComponent();
                        }
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddMarkupContent(47, "\r\n            ");
                        __builder4.OpenComponent<Blazorise.Column>(48);
                        __builder4.AddAttribute(49, "ColumnSize", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.IFluentColumn>(
#nullable restore
#line 43 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                ColumnSize.IsHalf.OnDesktop.IsFull.OnTablet.IsFull.OnMobile

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(50, "class", "chart-js-custom");
                        __builder4.AddAttribute(51, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<ChartJs.Blazor.Chart>(52);
                            __builder5.AddAttribute(53, "Config", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<ChartJs.Blazor.Common.ConfigBase>(
#nullable restore
#line 44 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
                                              _piePartnerPostsContentTypeChartConfig

#line default
#line hidden
#nullable disable
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
            __builder.AddMarkupContent(54, "\r\n");
            __builder.OpenComponent<Blazorise.Card>(55);
            __builder.AddAttribute(56, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.CardBody>(57);
                __builder2.AddAttribute(58, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenElement(59, "h3");
#nullable restore
#line 51 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
__builder3.AddContent(60, L["Partner.Dashboard.Campaigns"]);

#line default
#line hidden
#nullable disable
                    __builder3.CloseElement();
                    __builder3.AddMarkupContent(61, "\r\n        ");
                    __builder3.OpenComponent<FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components.HomePageCampaigns>(62);
                    __builder3.CloseComponent();
                }
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(63, "\r\n\r\n\r\n\r\n");
            __builder.OpenComponent<Blazorise.Card>(64);
            __builder.AddAttribute(65, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.CardHeader>(66);
                __builder2.AddAttribute(67, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenElement(68, "h3");
#nullable restore
#line 87 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\PartnerModule\Components\HomePagePartner.razor"
__builder3.AddContent(69, L["PartnerModule.Dashboard.LatestPost"]);

#line default
#line hidden
#nullable disable
                    __builder3.CloseElement();
                }
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(70, "\r\n    ");
                __builder2.OpenComponent<FacebookCommunityAnalytics.Api.Blazor.Pages.PartnerModule.Components.HomePagePosts>(71);
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IPartnerModuleAppService _partnerModuleAppService { get; set; }
    }
}
#pragma warning restore 1591
