using System;
using Volo.CmsKit.Pages;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{
    public class CmsPage : Page
    {
        public Guid SiteId { get; set; }
        public string LanguageCode { get; set; }
    }
}