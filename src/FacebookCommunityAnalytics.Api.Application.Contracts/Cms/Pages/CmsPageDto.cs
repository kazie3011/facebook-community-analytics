using System;
using FacebookCommunityAnalytics.Api.CmsSites;
using Volo.CmsKit.Admin.Pages;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{
    public class CmsPageDto : PageDto
    {
        public Guid SiteId { get; set; }
        public CmsSiteDto SiteDto { get; set; }
        public string LanguageCode { get; set; }
    }
}