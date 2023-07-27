using Blazorise;
using Castle.Core.Internal;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Blazor.Helpers;
using FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Flurl.Util;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using static System.Int32;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared
{
    public partial class TiktokModals: BlazorComponentBase
    {
        [Parameter]
        public TiktokDailyReports ContainerBlazorPage { get; set; }
        [Parameter]
        public GetTiktoksInputExtend Filter { get; set; }
        [Parameter]
        public string CurrentSorting { get; set; }

        private Modal ExportPostModal { get; set; }
        private string ExportPostFileName { get; set; }
        private bool isExportAll { get; set; } = false;
        // protected override async Task OnInitializedAsync()
        // {
        //     await SetToolbarItemsAsyncExtend();
        // }
        //
        // protected virtual ValueTask SetToolbarItemsAsyncExtend()
        // {
        //     ContainerBlazorPage.Toolbar.AddButton(L["ExportExcel"], () =>
        //     {
        //         OpenExportPostModal();
        //         return Task.CompletedTask;
        //     }, IconName.FilePdf, requiredPolicyName: ApiPermissions.Posts.Default);
        //
        //     return ValueTask.CompletedTask;
        // }
        private void OpenExportPostModal()
        {
            ExportPostModal.Show();
        }

        private void CloseExportPostModal()
        {
            ExportPostModal.Hide();
        }

        private async Task ExportPostsAsync()
        {
            Filter.Sorting = CurrentSorting;
            if (isExportAll)
            {
                Filter.MaxResultCount = MaxValue;
            }
            var posts = await TtiktokStatsAppService.GetExportRows(Filter);

            var excelBytes = ExportHelper.GenerateTiktokExcelBytes(L,posts, "Tiktoks");

            var fileName = ExportPostFileName.IsNullOrWhiteSpace() ? "Tiktoks" : ExportPostFileName;
            await js.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));

            ExportPostFileName = string.Empty;
            ExportPostModal.Hide();
        }

        private void OnExportAllChange(bool value)
        {
            isExportAll = value;
        }
    }
}
