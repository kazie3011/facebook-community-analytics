#pragma checksum "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6cdabf13027d00db5ddeef2c6d6d59ce01f6f18b"
// <auto-generated/>
#pragma warning disable 1591
namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
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
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.Permissions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.Tiktoks;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FA = Blazorise.Icons.FontAwesome;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.TrendingDetails;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using Volo.Abp.AspNetCore.Components.Web.Theming.Layout;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.Core.Const;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.Core.Enums;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using FacebookCommunityAnalytics.Api.Core.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
using Volo.Abp.AspNetCore.Components.Web.LeptonTheme.Components;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
           [Authorize(ApiPermissions.Tiktok.Dashboard)]

#line default
#line hidden
#nullable disable
    public partial class TikTokTrending : BlazorComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "row");
            __builder.OpenComponent<Blazorise.Field>(1);
            __builder.AddAttribute(2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.Form>(3);
                __builder2.AddAttribute(4, "id", "ContractSearchForm");
                __builder2.AddAttribute(5, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<Blazorise.Addons>(6);
                    __builder3.AddAttribute(7, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
                        __builder4.OpenComponent<Blazorise.Addon>(8);
                        __builder4.AddAttribute(9, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 23 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                  AddonType.Start

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(10, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<Blazorise.AddonLabel>(11);
                            __builder5.AddAttribute(12, "Class", "bg-primary text-white");
                            __builder5.AddAttribute(13, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder6) => {
#nullable restore
#line 24 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder6.AddContent(14, L["Icon.Date"]);

#line default
#line hidden
#nullable disable
                            }
                            ));
                            __builder5.CloseComponent();
                        }
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddMarkupContent(15, "\r\n                ");
                        __builder4.OpenComponent<Blazorise.Addon>(16);
                        __builder4.AddAttribute(17, "AddonType", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.AddonType>(
#nullable restore
#line 26 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                  AddonType.Start

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(18, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder5) => {
                            __builder5.OpenComponent<Blazorise.DatePicker<DateTimeOffset?>>(19);
                            __builder5.AddAttribute(20, "Date", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<DateTimeOffset?>(
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                DateFilter

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(21, "DateChanged", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<DateTimeOffset?>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<DateTimeOffset?>(this, 
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                         OnDate_Changed

#line default
#line hidden
#nullable disable
                            )));
                            __builder5.AddAttribute(22, "DisplayFormat", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                         GlobalConsts.DateFormat

#line default
#line hidden
#nullable disable
                            ));
                            __builder5.AddAttribute(23, "placeholder", 
#nullable restore
#line 27 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                                                L["SelectDates"]

#line default
#line hidden
#nullable disable
                            );
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
            __builder.CloseElement();
            __builder.AddMarkupContent(24, "\r\n");
            __builder.OpenElement(25, "row");
#nullable restore
#line 34 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
     if (TopTrendingDetails.IsNotNullOrEmpty())
    {

#line default
#line hidden
#nullable disable
            __builder.OpenElement(26, "table");
            __builder.AddAttribute(27, "class", "b-table table-striped table-sm");
            __builder.AddAttribute(28, "style", "width:100%");
            __builder.OpenComponent<Blazorise.TableHeader>(29);
            __builder.AddAttribute(30, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Blazorise.TableRow>(31);
                __builder2.AddAttribute(32, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 38 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                         TextAlignment.Center

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(33, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<Blazorise.TableHeaderCell>(34);
                    __builder3.AddAttribute(35, "Style", "background-color: whitesmoke;vertical-align:middle;width: 100px;font-size: 14px");
                    __builder3.AddAttribute(36, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 39 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                            TextAlignment.Center

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(37, "Clicked", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Blazorise.BLMouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Blazorise.BLMouseEventArgs>(this, 
#nullable restore
#line 39 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                                                           () =>{TiktokOrderType(TiktokTrendingProperty.Rank);}

#line default
#line hidden
#nullable disable
                    )));
                    __builder3.AddAttribute(38, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 40 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(39, L["TikTokTrending.Rank"]);

#line default
#line hidden
#nullable disable
                        __builder4.AddMarkupContent(40, "\r\n                        ");
                        __builder4.OpenComponent<Blazorise.Icon>(41);
                        __builder4.AddAttribute(42, "TextColor", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextColor>(
#nullable restore
#line 41 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                         TextColor.Dark

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(43, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 41 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                 VisibleRankProperty ? Icon : null

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(44, "\r\n                    ");
                    __builder3.OpenComponent<Blazorise.TableHeaderCell>(45);
                    __builder3.AddAttribute(46, "Style", "background-color: whitesmoke;vertical-align:middle;font-size: 14px");
                    __builder3.AddAttribute(47, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 43 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                               TextAlignment.Left

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(48, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 44 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(49, L["TikTokTrending.TrendingContent"]);

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(50, "\r\n                    ");
                    __builder3.OpenComponent<Blazorise.TableHeaderCell>(51);
                    __builder3.AddAttribute(52, "Style", "background-color: whitesmoke;vertical-align:middle;font-size: 14px");
                    __builder3.AddAttribute(53, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 46 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                               TextAlignment.Center

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(54, "Clicked", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Blazorise.BLMouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Blazorise.BLMouseEventArgs>(this, 
#nullable restore
#line 46 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                                              () =>{TiktokOrderType(TiktokTrendingProperty.View);}

#line default
#line hidden
#nullable disable
                    )));
                    __builder3.AddAttribute(55, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 47 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(56, L["TikTokTrending.View"]);

#line default
#line hidden
#nullable disable
                        __builder4.AddMarkupContent(57, "\r\n                        ");
                        __builder4.OpenComponent<Blazorise.Icon>(58);
                        __builder4.AddAttribute(59, "TextColor", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextColor>(
#nullable restore
#line 48 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                         TextColor.Dark

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(60, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 48 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                 VisibleViewProperty ? Icon : null

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(61, "\r\n                    ");
                    __builder3.OpenComponent<Blazorise.TableHeaderCell>(62);
                    __builder3.AddAttribute(63, "Style", "background-color: whitesmoke;vertical-align:middle;font-size: 14px;");
                    __builder3.AddAttribute(64, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 50 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                TextAlignment.Left

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(65, "Clicked", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Blazorise.BLMouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Blazorise.BLMouseEventArgs>(this, 
#nullable restore
#line 50 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                                                                                             () =>{TiktokOrderType(TiktokTrendingProperty.Increase);}

#line default
#line hidden
#nullable disable
                    )));
                    __builder3.AddAttribute(66, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
                        __builder4.OpenElement(67, "div");
                        __builder4.AddAttribute(68, "class", "rank-tooltip");
#nullable restore
#line 52 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(69, L["TikTokTrending.RankChange"]);

#line default
#line hidden
#nullable disable
                        __builder4.AddMarkupContent(70, "\r\n                            ");
                        __builder4.OpenComponent<Blazorise.Icon>(71);
                        __builder4.AddAttribute(72, "TextColor", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextColor>(
#nullable restore
#line 53 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                             TextColor.Dark

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(73, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 53 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                     VisibleInCreaseProperty ? Icon : null

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddMarkupContent(74, "\r\n                            ");
                        __builder4.OpenElement(75, "span");
                        __builder4.AddAttribute(76, "class", "text-tooltip");
#nullable restore
#line 54 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(77, (MarkupString)L["TiktokTrending.Tooltip.Increase"].ToString());

#line default
#line hidden
#nullable disable
                        __builder4.CloseElement();
                        __builder4.CloseElement();
                    }
                    ));
                    __builder3.CloseComponent();
                }
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(78, "\r\n\r\n            ");
            __builder.OpenComponent<Blazorise.TableBody>(79);
            __builder.AddAttribute(80, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
#nullable restore
#line 61 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                 foreach (var item in TopTrendingDetails)
                {

#line default
#line hidden
#nullable disable
                __builder2.OpenComponent<Blazorise.TableRow>(81);
                __builder2.AddAttribute(82, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenComponent<Blazorise.TableRowCell>(83);
                    __builder3.AddAttribute(84, "Style", "font-size: 14px;font-weight: 500;");
                    __builder3.AddAttribute(85, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 64 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                               TextAlignment.Center

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(86, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 64 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(87, item.Rank);

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(88, "\r\n                        ");
                    __builder3.OpenComponent<Blazorise.TableRowCell>(89);
                    __builder3.AddAttribute(90, "Style", "font-size: 14px;font-weight: 500;");
                    __builder3.AddAttribute(91, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 65 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(92, item.Description);

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(93, "\r\n                        ");
                    __builder3.OpenComponent<Blazorise.TableRowCell>(94);
                    __builder3.AddAttribute(95, "Style", "font-size: 14px;font-weight: 500;");
                    __builder3.AddAttribute(96, "TextAlignment", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextAlignment>(
#nullable restore
#line 66 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                               TextAlignment.Center

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(97, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 67 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(98, item.View.ToCommaStyle("0"));

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
                    __builder3.AddMarkupContent(99, "\r\n                        ");
                    __builder3.OpenComponent<Blazorise.TableRowCell>(100);
                    __builder3.AddAttribute(101, "Style", "font-size: 16px;font-weight: 500;");
                    __builder3.AddAttribute(102, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 70 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                             switch (item.Increase)
                            {
                                case >= 0:

#line default
#line hidden
#nullable disable
                        __builder4.OpenElement(103, "div");
                        __builder4.AddAttribute(104, "class", "text-success");
                        __builder4.OpenComponent<Blazorise.Icon>(105);
                        __builder4.AddAttribute(106, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 74 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                    IconName.ArrowUp

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(107, "TextColor", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextColor>(
#nullable restore
#line 74 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                 TextColor.Success

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddContent(108, " ");
#nullable restore
#line 74 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(109, item.Increase == 0 ? string.Empty : Math.Abs(item.Increase));

#line default
#line hidden
#nullable disable
                        __builder4.CloseElement();
#nullable restore
#line 76 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                    break;
                                case < 0:

#line default
#line hidden
#nullable disable
                        __builder4.OpenElement(110, "div");
                        __builder4.AddAttribute(111, "class", "text-danger");
                        __builder4.OpenComponent<Blazorise.Icon>(112);
                        __builder4.AddAttribute(113, "Name", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 79 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                    IconName.ArrowDown

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(114, "TextColor", global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Blazorise.TextColor>(
#nullable restore
#line 79 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                                                                   TextColor.Danger

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
                        __builder4.AddContent(115, " ");
#nullable restore
#line 79 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
__builder4.AddContent(116, Math.Abs(item.Increase));

#line default
#line hidden
#nullable disable
                        __builder4.CloseElement();
#nullable restore
#line 81 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                                    break;
                                default:
                                    break;
                            }

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
                }
                ));
                __builder2.CloseComponent();
#nullable restore
#line 87 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
                }

#line default
#line hidden
#nullable disable
            }
            ));
            __builder.CloseComponent();
            __builder.CloseElement();
#nullable restore
#line 90 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\TikTok\Dashboards\TikTokTrending.razor"
    }

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IMessageService MessageService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ITiktokStatsAppService _tiktokStatsAppService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime JSRuntime { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager NavigationManager { get; set; }
    }
}
#pragma warning restore 1591