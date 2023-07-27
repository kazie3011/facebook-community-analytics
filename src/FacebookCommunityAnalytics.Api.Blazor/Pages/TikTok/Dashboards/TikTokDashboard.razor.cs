using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Markdig.Extensions.TaskLists;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
{
    public partial class TikTokDashboard
    {
        private List<BreadcrumbItem> BreadcrumbItems = new();
        private PageToolbar Toolbar { get; } = new();
        private string selectedTabName = "MCNVietNam";
        private MCNVietNamTiktokDashboard _mcnVietNamTiktokDashboard;
        private MCNGDLTiktokDashboard _mcngdlTiktokDashboard;
        private MCNInternalKPIDashboard _mcnInternalChannelTiktokDashboard;
        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetBreadcrumbItemsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage(L["TikTok.Dashboard.Title"].Value);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Tiktok"]));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:TiktokDashboard"]));
            return ValueTask.CompletedTask;
        }

        private async Task OnSelectedTabChanged(string name)
        {
            selectedTabName = name;
            await InvokeAsync(StateHasChanged);
        }

        protected override async void Dispose(bool disposing)
        {
            if (disposing)
            {
                await Task.Delay(500);
                await JsRuntime.InvokeVoidAsync("generalInterop.clearTooltips");
            }

            base.Dispose(disposing);
        }

    }
}