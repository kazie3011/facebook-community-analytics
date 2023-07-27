using System.Threading.Tasks;
using Microsoft.JSInterop;
using ChartJsColor = System.Drawing.Color;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages
{
    public partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            // if (!CurrentUser.IsAuthenticated)
            // {
            //    
            // }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                await JsRuntime.InvokeVoidAsync("RedirectToLoginPage");
            }
        }
    }
}