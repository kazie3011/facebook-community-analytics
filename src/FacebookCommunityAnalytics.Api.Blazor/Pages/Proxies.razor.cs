using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Proxies
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<ProxyDto> ProxyList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateProxy { get; set; }
        private bool CanEditProxy { get; set; }
        private bool CanDeleteProxy { get; set; }
        private ProxyCreateDto NewProxy { get; set; }
        private Validations NewProxyValidations { get; set; }
        private ProxyUpdateDto EditingProxy { get; set; }
        private Validations EditingProxyValidations { get; set; }
        private Guid EditingProxyId { get; set; }
        private Modal CreateProxyModal { get; set; }
        private Modal EditProxyModal { get; set; }
        private GetProxiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<ProxyDto> EntityActionsColumn { get; set; }
        private Modal ImportProxyModal { get; set; }
        private List<ProxyImportDto> ProxyImportDtos;

        public Proxies()
        {
            NewProxy = new ProxyCreateDto();
            EditingProxy = new ProxyUpdateDto();
            Filter = new GetProxiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            BrowserDateTime = await GetBrowserDateTime();
            await JSRuntime.InvokeVoidAsync("setTitle", $"GDL - {L["Proxies.PageTitle"].Value}");
            await JSRuntime.InvokeVoidAsync("HiddenMenuOnMobile");
            await InitPage($"GDL - {L["Proxies.PageTitle"].Value}");
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Proxies"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
            (
                L["NewProxy"],
                () =>
                {
                    OpenCreateProxyModal();
                    return Task.CompletedTask;
                },
                IconName.Add,
                requiredPolicyName: ApiPermissions.Proxies.Create
            );


            Toolbar.AddButton
            (
                L["Ping"],
                async () =>
                {
                    await ProxyExtendAppService.GetAlives();
                    await GetProxiesAsync();
                },
                IconName.FileUpload,
                requiredPolicyName: ApiPermissions.Proxies.Create
            );

            Toolbar.AddButton
            (
                L["ImportProxy"],
                () =>
                {
                    OpenImportProxyModal();
                    return Task.CompletedTask;
                },
                IconName.FileUpload,
                requiredPolicyName: ApiPermissions.Proxies.Create
            );

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Proxies.Create);
            CanEditProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Proxies.Edit);
            CanDeleteProxy = await AuthorizationService
                .IsGrantedAsync(ApiPermissions.Proxies.Delete);
        }

        private async Task GetProxiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ProxiesAppService.GetListAsync(Filter);
            ProxyList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetProxiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProxyDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetProxiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenCreateProxyModal()
        {
            NewProxy = new ProxyCreateDto
            {
                LastPingDateTime = DateTime.UtcNow,
            };
            NewProxyValidations.ClearAll();
            CreateProxyModal.Show();
        }

        private void CloseCreateProxyModal()
        {
            CreateProxyModal.Hide();
        }

        private void OpenEditProxyModal(ProxyDto input)
        {
            EditingProxyId = input.Id;
            EditingProxy = ObjectMapper.Map<ProxyDto, ProxyUpdateDto>(input);
            EditingProxyValidations.ClearAll();
            EditProxyModal.Show();
        }

        private async Task DeleteProxyAsync(ProxyDto input)
        {
            await ProxiesAppService.DeleteAsync(input.Id);
            await GetProxiesAsync();
        }

        private async Task CreateProxyAsync()
        {
            if (!NewProxyValidations.ValidateAll()) return;
            await ProxiesAppService.CreateAsync(NewProxy);
            await GetProxiesAsync();
            CreateProxyModal.Hide();
        }

        private void CloseEditProxyModal()
        {
            EditProxyModal.Hide();
        }

        private async Task UpdateProxyAsync()
        {
            if (!EditingProxyValidations.ValidateAll()) return;
            await ProxiesAppService.UpdateAsync(EditingProxyId, EditingProxy);
            await GetProxiesAsync();
            EditProxyModal.Hide();
        }

        private void OpenImportProxyModal()
        {
            ImportProxyModal.Show();
        }

        private void CloseImportProxyModal()
        {
            ImportProxyModal.Hide();
        }

        private async Task OnFileImportProxiesChanged(FileChangedEventArgs e)
        {
            ProxyImportDtos = new List<ProxyImportDto>();
            foreach (var file in e.Files)
            {
                await using var stream = new MemoryStream();

                await file.WriteToStreamAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var excel = new ExcelPackage(stream);
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();

                var postImportItems = workSheet.ConvertSheetToObjects<ProxyImportDto>().ToList();

                if (postImportItems.Any())
                {
                    ProxyImportDtos.AddRange(postImportItems);
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task ImportProxiesAsync()
        {
            var proxies = ProxyImportDtos.Where(x => x.Ip.IsNotNullOrWhiteSpace()).ToList();
            await ProxyExtendAppService.ImportProxiesAsync(new ProxyImportInput {Items = proxies});
            await GetProxiesAsync();
            ImportProxyModal.Hide();
            ProxyImportDtos = new List<ProxyImportDto>();
        }
    }
}