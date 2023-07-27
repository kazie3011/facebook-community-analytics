using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Groups;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.CmsSites
{
    public class CmsSiteDto : AuditedEntityDto<Guid>
    {
        public CmsSiteDto()
        {
            SiteSeo = new SiteSEODto();
            FooterSite = new FooterSiteDto();
        }
        public Guid? GroupId { get; set; }
        public GroupDto Group { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string FaviconUrl { get; set; }
        public bool IsActive { get; set; }
        
        public SiteSEODto SiteSeo { get; set; }
        public FooterSiteDto FooterSite { get; set; }
    }
    public class FooterSiteDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
    }
    
    public class SiteSEODto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string Keywords { get; set; }
        public string ImageUrl { get; set; }
    }
}