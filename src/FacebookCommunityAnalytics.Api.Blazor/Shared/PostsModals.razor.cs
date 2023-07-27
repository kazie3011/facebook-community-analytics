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
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Flurl.Util;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Shared
{
    public partial class PostsModals: BlazorComponentBase
    {
        [Parameter]
        public Pages.Posts ContainerBlazorPage { get; set; }
        
        [Parameter]
        public GetPostsInputExtend Filter { get; set; }
        
        [Parameter]
        public string CurrentSorting { get; set; }
        private Modal ImportPostModal { get; set; }
        private List<PostImportDto> PostImportDtos { get; set; }
        private Modal ExportPostModal { get; set; }
        private string ExportPostFileName { get; set; }
        private List<LookupDto<int>> FileSheetDtos { get; set; } = new();
        
        private IFileEntry CurrentFileImport = null;
        private bool isExportAll { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsyncExtend();
        }

        protected virtual ValueTask SetToolbarItemsAsyncExtend()
        {
            ContainerBlazorPage.Toolbar.AddButton(L["ExportExcel"], () =>
            {
                OpenExportPostModal();
                return Task.CompletedTask;
            }, IconName.FilePdf, requiredPolicyName: ApiPermissions.Posts.Default);

            return ValueTask.CompletedTask;
        }

        public void OpenImportModal()
        {
            ImportPostModal.Show();
        }
        private void CloseImportPostModal()
        {
            ImportPostModal.Hide();
        }

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

            var posts = await PostsExtendAppService.GetPostDetailExportRow(Filter);

            foreach (var post in posts)
            {
                // post.PostCopyrightType = L[$"Enum:PostCopyrightType:{(int)post.PostCopyrightType.ToEnumerable<PostCopyrightType>()}"];
                post.IsNotAvailable = post.IsNotAvailable == PostConsts.True ? L["Post.IsNotAvailable"] : L["Post.IsAvailable"];
            }
            var excelBytes = GenerateExcelBytes(posts, "Posts");

            var fileName = ExportPostFileName.IsNullOrWhiteSpace() ? "Posts" : ExportPostFileName;
            await JsRuntime.InvokeVoidAsync("saveAsFile", $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", Convert.ToBase64String(excelBytes));

            ExportPostFileName = string.Empty;
            ExportPostModal.Hide();
        }

        private async Task OnFileChanged(FileChangedEventArgs e)
        {
            FileSheetDtos.Clear();
            PostImportDtos = new List<PostImportDto>();
            foreach (var file in e.Files)
            {
                // A stream is going to be the destination stream we're writing to.                
                await using var stream = new MemoryStream();
                // Here we're telling the FileEdit where to write the upload result
                await file.WriteToStreamAsync(stream);

                // Once we reach this line it means the file is fully uploaded.
                // In this case we're going to offset to the beginning of file
                // so we can read it.
                stream.Seek(0, SeekOrigin.Begin);
                var excel = new ExcelPackage(stream);

                foreach (var worksheet in excel.Workbook.Worksheets)
                {
                    FileSheetDtos.Add(new LookupDto<int>()
                    {
                        Id = worksheet.Index,
                        DisplayName = worksheet.Name
                    });
                }

                CurrentFileImport = file;
            }

            await InvokeAsync(StateHasChanged);
        }
        
        private byte[] GenerateExcelBytes<T>(List<T> rows, string sheetName = "Sheet1") where T : class
        {
            // Creating an instance
            // of ExcelPackage
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(rows, true);

            int i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<T>();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{typeof(T).Name}:{header}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            // Link
            workSheet.Column(3).AutoFit(0, 80);
            // Like Comment Share Total
            workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // IsNotAvailable
            workSheet.Column(12).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(12).AutoFit(16, 20);
            workSheet.Column(13).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(13).AutoFit(16, 20);
            workSheet.Column(14).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(14).AutoFit(16, 20);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        private void OnExportAllChange(bool value)
        {
            isExportAll = value;
        }
    }
}
