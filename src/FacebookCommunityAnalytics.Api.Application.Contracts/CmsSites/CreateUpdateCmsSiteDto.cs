using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace FacebookCommunityAnalytics.Api.CmsSites
{
    public class CreateUpdateCmsSiteDto
    {
        public CreateUpdateCmsSiteDto()
        {
            SiteSeo = new SiteSEODto();
            FooterSite = new FooterSiteDto();
        }
        public Guid? GroupId { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string FaviconUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public SiteSEODto SiteSeo { get; set; }
        public FooterSiteDto FooterSite { get; set; }
    }
}