using Blazorise.RichTextEdit;
using FacebookCommunityAnalytics.Api.Blazor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<ApiBlazorModule>();
            services.AddBlazoriseRichTextEdit();
            services.AddScoped<ClipboardService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.InitializeApplication();
        }
    }
}
