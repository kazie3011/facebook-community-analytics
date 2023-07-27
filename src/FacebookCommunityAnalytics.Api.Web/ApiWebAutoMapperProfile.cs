using AutoMapper;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Web
{
    public class ApiWebAutoMapperProfile : Profile
    {
        public ApiWebAutoMapperProfile()
        {
            //Define your AutoMapper configuration here for the Web project.
            
            CreateMap<PartnerDto, PartnerUpdateDto>();

            CreateMap<CampaignDto, CampaignUpdateDto>();

            CreateMap<PostDto, PostUpdateDto>();
            CreateMap<PostDto, PostUpdateNoteDto>();
        }
    }
}
