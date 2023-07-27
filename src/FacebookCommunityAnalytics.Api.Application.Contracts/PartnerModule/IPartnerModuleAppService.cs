using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.PartnerModule
{
    public interface IPartnerModuleAppService : IApplicationService
    {
        Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostNavs(GetPostsInputExtend input);
        Task<PagedResultDto<GroupDto>> GetGroups(GetGroupsInput input);
        Task<PagedResultDto<PartnerDto>> GetPartners(GetPartnersInput input);
        Task<PartnerDto> GetPartnerById(Guid id);
        Task<CampaignDto> GetCampaign(Guid id);
        Task<List<CampaignDto>> GetCampaigns(GetCampaignsInput input);
        Task<List<CampaignPostDto>> GetCampaignPosts(Guid campaignId);
        Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetPartnerCampaigns(GetCampaignsInput input);
        Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetCampaignNavs(GetCampaignsInput input);
        Task<List<LookupDto<Guid>>> GetPartnerUserLookup(LookupRequestDto input);
        Task<List<LookupDto<Guid?>>> GetPartnersLookup(LookupRequestDto input);
        Task<List<LookupDto<Guid?>>> GetCampaignLookup(GetCampaignLookupDto input);
        Task<List<LookupDto<Guid?>>> GetGroupLookup(GroupLookupRequestDto input);
        Task<List<LookupDto<Guid?>>> GetCategoryLookup(LookupRequestDto input);
        Task<List<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input);
        Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractNavs(GetContractsInput input);
        Task<bool> ContractExist(string contractCode);
        Task<List<LookupDto<Guid?>>> GetUserLookup(GetMembersApiRequest input);
        Task<ContractDto> CreateContract(CreateUpdateContractDto input);
        Task<CampaignDto> CreateCampaign(CampaignCreateDto input);
        Task<PartnerDto> CreatePartner(PartnerCreateDto input);
        Task<ContractDto> EditContract(Guid id, CreateUpdateContractDto input);
        Task<CampaignDto> EditCampaign(Guid id, CampaignUpdateDto input);
        Task<PartnerDto> EditPartner(Guid id, PartnerUpdateDto input);
        Task DeleteContract(Guid id);
        Task DeleteCampaign(Guid id);
        Task DeletePartner(Guid id);
        Task SendCampaignEmail(Guid campaignId);
        Task<byte[]> ExportCampaign(Guid campaignId);
        Task RemoveCampaignPost(Guid postId);
        Task CreateCampaignPosts(PostCreateDto input);

        Task<GroupDto> CreateGroup(GroupCreateDto groupCreateDto);
        Task<GroupDto> UpdateGroupAsync(Guid id, GroupUpdateDto groupUpdateDto);

        Task<GrowthCampaignChartDto> GetGrowthCampaignChart(GetGrowthCampaignChartsInput input);

        Task<PostDto> CreatePost(PostCreateDto input);
        Task CreateMultiplePosts(PostCreateDto input);

        Task<PostDto> UpdatePost(Guid id, PostUpdateDto input);
        Task DeletePost(Guid id);

        Task<List<PartnerDto>> GetPartnersByUser(Guid userId);
        Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input);
        Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetTikToksPaging(GetTiktoksInputExtend input);
        Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id);

        //Partner Users
        Task<List<AppUserDto>> GetPartnerUsers();
        Task AddPartnerUser(string email, string name, string surname);

        Task<bool> UserExist(string email);

        Task RemovePartnerUser(Guid userId);

        Task<PartnerCampaignChartDto> GetPartnerCampaignsChart();
        Task<PartnerPostTypeChartDto> GetPartnerPostContentTypeChart(DateTime startDate, DateTime endDate);

        Task<GrowthCampaignChartDto> GetGrowthCampaignChartsAsync(GetGrowthCampaignChartsInput input);

        Task<PostDto> GetPostAsync(Guid postId);
        Task<PostDto> UpdateNotePost(Guid id, PostUpdateNoteDto input);
    }
}