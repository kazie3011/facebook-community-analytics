// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

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
#line 3 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Posts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using Volo.Abp.AspNetCore.Components.Messages;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using Volo.Abp.AspNetCore.Components.Web.Theming.Layout;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Permissions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Core.Enums;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Core.Const;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Core.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.Groups;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FA = Blazorise.Icons.FontAwesome;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using FacebookCommunityAnalytics.Api.UserInfos;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
using Faso.Blazor.SpinKit;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src\FacebookCommunityAnalytics.Api.Blazor\Pages\Posts.razor"
           [Authorize(ApiPermissions.Posts.Default)]

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/posts")]
    public partial class Posts : BlazorComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager NavigationManager { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IUserInfosAppService UserInfosAppService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IUiMessageService UiMessageService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IGroupExtendAppService GroupExtendAppService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IPostsExtendAppService PostsExtendAppService { get; set; }
    }
}
#pragma warning restore 1591
