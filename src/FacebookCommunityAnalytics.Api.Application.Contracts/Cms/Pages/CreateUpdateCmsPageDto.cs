using System;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{
    public class CreateUpdateCmsPageDto
    {
        public string Title { get; set; }

        public string Slug { get; set; }
        
        public string Content { get; set; }

        public string Script { get; set; }

        public string Style { get; set; }
        
        public Guid SiteId { get; set; }
        public string LanguageCode { get; set; }
    }
}