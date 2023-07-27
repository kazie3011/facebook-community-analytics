using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Medias
{
    public class GetMediasInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public MediaCategory? MediaCategory { get; set; }   
    }
}