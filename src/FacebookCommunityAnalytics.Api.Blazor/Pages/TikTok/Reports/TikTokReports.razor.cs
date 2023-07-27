using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports
{
    public partial class TikTokReports
    {
        private List<BreadcrumbItem> BreadcrumbItems = new();
        private PageToolbar Toolbar { get; } = new();
        private string selectedTabName = "TikTokDailyReport";
        private TiktokDailyReports _tiktokDailyReports;
        private TiktokWeeklyReports _tiktokWeeklyReports;
        private TiktokMonthlyReports _tiktokMonthlyReports;
        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetBreadcrumbItemsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage(L["TikTok.TikTokReport.Title"].Value);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Home"], "/"));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Tiktok"]));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:TiktokDailyReports"]));
            return ValueTask.CompletedTask;
        }
        private async Task OnSelectedTabChanged(string name)
        {
            selectedTabName = name;
            await InvokeAsync(StateHasChanged);
        }


    }
}