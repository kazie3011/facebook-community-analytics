using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class AccountProxies
    {
        private async Task Init()
        {
            await GetNullableAccountLookupAsync(string.Empty);
            await GetNullableProxyLookupAsync(string.Empty);
        }

        private void SetToolbarItemsExtendAsync()
        {
            Toolbar.AddButton
            (
                L["RebindAccountProxy"],
                async () =>
                {
                    await AccountProxiesExtendAppService.RebindAccountProxies();
                    await GetAccountProxiesAsync();
                },
                IconName.Circle,
                requiredPolicyName: ApiPermissions.AccountProxies.Create
            );
        }
    }
}