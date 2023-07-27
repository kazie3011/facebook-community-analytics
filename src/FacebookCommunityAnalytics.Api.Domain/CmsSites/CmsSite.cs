using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.CmsSites
{
    public class CmsSite : AuditedEntity<Guid>
    {
        public CmsSite()
        {
            SiteSeo = new SiteSEO();
            FooterSite = new FooterSite();
        }
        public Guid? GroupId { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string FaviconUrl { get; set; }
        public bool IsActive { get; set; }
        
        public SiteSEO SiteSeo { get; set; }
        public FooterSite FooterSite { get; set; }
    }

    public class FooterSite
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
    }
    
    public class SiteSEO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string ImageUrl { get; set; }
    }
}