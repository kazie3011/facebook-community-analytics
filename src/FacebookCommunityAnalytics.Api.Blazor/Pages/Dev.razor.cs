using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using FacebookCommunityAnalytics.Api.Dev;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Dev
    {
        protected PageToolbar Toolbar { get; } = new();
        private List<string> NoRoleUsers = new List<string>();
        private List<string> NoTeamUsers = new List<string>();
        private List<string> NoPostUsers = new List<string>();
        private List<string> DuplicatedUsers = new List<string>();
        private List<PostWithNavigationPropertiesDto> InvalidUrlsPosts = new List<PostWithNavigationPropertiesDto>();
        private DevResponse<string> NoCreatedAtPosts = new DevResponse<string>();
        private int UncrawledPosts = new int();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private string NewDevPostResponse { get; set; }
        private string Filter { get; set; }


        public Dev()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await GetUserResponseAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InitPage($"GDL - {L["Dev.PageTitle"].Value}");
        }

        private async Task GetUserResponseAsync()
        {
            NoRoleUsers = await DevAppService.Users_GetNoRoles();
            NoTeamUsers = await DevAppService.Users_GetNoTeams();
            NoPostUsers = await DevAppService.Users_GetNoPosts();
            DuplicatedUsers = await DevAppService.Users_GetDuplicated();
            NoCreatedAtPosts = await DevAppService.Posts_GetNoCreatedAt();
            InvalidUrlsPosts = await DevAppService.Posts_GetInvalidUrls();
            UncrawledPosts = await DevAppService.Crawl_InitUncrawledPosts();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<string> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserResponseAsync();
        }
    }
}