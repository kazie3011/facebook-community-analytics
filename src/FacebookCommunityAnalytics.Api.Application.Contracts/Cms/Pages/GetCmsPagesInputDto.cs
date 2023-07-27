using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{
    public class GetCmsPagesInputDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}