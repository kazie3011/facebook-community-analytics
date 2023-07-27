using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Controllers.PartnerModules
{
    [RemoteService]
    [ControllerName("PartnerModule")]
    [Route("api/app/tools")]
    public class PartnerModuleController : ApiController, IPartnerModuleAppService
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public PartnerModuleController(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        [HttpGet]
        [Route("get-post-navs")]
        public Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostNavs(GetPostsInputExtend input)
        {
            return _partnerModuleAppService.GetPostNavs(input);
        }

        [HttpGet]
        [Route("get-groups")]
        public Task<PagedResultDto<GroupDto>> GetGroups(GetGroupsInput input)
        {
            return _partnerModuleAppService.GetGroups(input);
        }

        [HttpGet]
        [Route("get-partners")]
        public Task<PagedResultDto<PartnerDto>> GetPartners(GetPartnersInput input)
        {
            return _partnerModuleAppService.GetPartners(input);
        }

        [HttpGet]
        [Route("get-partner-by-id")]
        public Task<PartnerDto> GetPartnerById(Guid id)
        {
            return _partnerModuleAppService.GetPartnerById(id);
        }

        [HttpGet]
        [Route("get-campaign")]
        public Task<CampaignDto> GetCampaign(Guid id)
        {
            return _partnerModuleAppService.GetCampaign(id);
        }

        [HttpGet]
        [Route("get-campaigns")]
        public Task<List<CampaignDto>> GetCampaigns(GetCampaignsInput input)
        {
            return _partnerModuleAppService.GetCampaigns(input);
        }

        [HttpGet]
        [Route("get-campaign-posts")]
        public Task<List<CampaignPostDto>> GetCampaignPosts(Guid campaignId)
        {
            return _partnerModuleAppService.GetCampaignPosts(campaignId);
        }

        [HttpGet]
        [Route("get-partner-campaigns")]
        public Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetPartnerCampaigns(GetCampaignsInput input)
        {
            return _partnerModuleAppService.GetPartnerCampaigns(input);
        }

        [HttpGet]
        [Route("get-campaign-navs")]
        public Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetCampaignNavs(GetCampaignsInput input)
        {
            return _partnerModuleAppService.GetCampaignNavs(input);
        }

        [HttpGet]
        [Route("get-partner-user-lookup")]
        public Task<List<LookupDto<Guid>>> GetPartnerUserLookup(LookupRequestDto input)
        {
            return _partnerModuleAppService.GetPartnerUserLookup(input);
        }

        [HttpGet]
        [Route("get-partner-lookup")]
        public Task<List<LookupDto<Guid?>>> GetPartnersLookup(LookupRequestDto input)
        {
            return _partnerModuleAppService.GetPartnersLookup(input);
        }

        [HttpGet]
        [Route("get-campaign-lookup")]
        public Task<List<LookupDto<Guid?>>> GetCampaignLookup(GetCampaignLookupDto input)
        {
            return _partnerModuleAppService.GetCampaignLookup(input);
        }

        [HttpGet]
        [Route("get-groups-lookup")]
        public Task<List<LookupDto<Guid?>>> GetGroupLookup(GroupLookupRequestDto input)
        {
            return _partnerModuleAppService.GetGroupLookup(input);
        }

        [HttpGet]
        [Route("get-category-lookup")]
        public Task<List<LookupDto<Guid?>>> GetCategoryLookup(LookupRequestDto input)
        {
            return _partnerModuleAppService.GetCategoryLookup(input);
        }

        [HttpGet]
        [Route("get-running-campaigns-lookup")]
        public Task<List<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input)
        {
            return _partnerModuleAppService.GetRunningCampaignLookup(input);
        }

        [HttpGet]
        [Route("get-contract-nav")]
        public Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractNavs(GetContractsInput input)
        {
            return _partnerModuleAppService.GetContractNavs(input);
        }

        [HttpGet]
        [Route("contract-exist/{contractCode}")]
        public Task<bool> ContractExist(string contractCode)
        {
            return _partnerModuleAppService.ContractExist(contractCode);
        }

        [HttpGet]
        [Route("get-app-user-lookup")]
        public Task<List<LookupDto<Guid?>>> GetUserLookup(GetMembersApiRequest input)
        {
            return _partnerModuleAppService.GetUserLookup(input);
        }

        [HttpPost]
        [Route("create-contract")]
        public Task<ContractDto> CreateContract(CreateUpdateContractDto input)
        {
            return _partnerModuleAppService.CreateContract(input);
        }

        [HttpPost]
        [Route("create-campaign")]
        public Task<CampaignDto> CreateCampaign(CampaignCreateDto input)
        {
            return _partnerModuleAppService.CreateCampaign(input);
        }

        [HttpPost]
        [Route("create-partner")]
        public Task<PartnerDto> CreatePartner(PartnerCreateDto input)
        {
            return _partnerModuleAppService.CreatePartner(input);
        }

        [HttpPut]
        [Route("edit-contract")]
        public Task<ContractDto> EditContract(Guid id, CreateUpdateContractDto input)
        {
            return _partnerModuleAppService.EditContract(id, input);
        }

        [HttpPut]
        [Route("edit-campaign")]
        public Task<CampaignDto> EditCampaign(Guid id, CampaignUpdateDto input)
        {
            return _partnerModuleAppService.EditCampaign(id, input);
        }

        [HttpPut]
        [Route("edit-partner")]
        public Task<PartnerDto> EditPartner(Guid id, PartnerUpdateDto input)
        {
            return _partnerModuleAppService.EditPartner(id, input);
        }

        [HttpDelete]
        [Route("delete-contract")]
        public Task DeleteContract(Guid id)
        {
            return _partnerModuleAppService.DeleteContract(id);
        }

        [HttpDelete]
        [Route("delete-campaign")]
        public Task DeleteCampaign(Guid id)
        {
            return _partnerModuleAppService.DeleteCampaign(id);
        }

        [HttpDelete]
        [Route("delete-partner")]
        public Task DeletePartner(Guid id)
        {
            return _partnerModuleAppService.DeletePartner(id);
        }

        [HttpGet]
        [Route("send-campaign-email")]
        public Task SendCampaignEmail(Guid campaignId)
        {
            return _partnerModuleAppService.SendCampaignEmail(campaignId);
        }

        [HttpGet]
        [Route("export-campaign")]
        public Task<byte[]> ExportCampaign(Guid campaignId)
        {
            return _partnerModuleAppService.ExportCampaign(campaignId);
        }

        [HttpDelete]
        [Route("remove-campaign-post/{postId}")]
        public Task RemoveCampaignPost(Guid postId)
        {
            return _partnerModuleAppService.RemoveCampaignPost(postId);
        }

        [HttpPost]
        [Route("create-campaign-post")]
        public Task CreateCampaignPosts(PostCreateDto input)
        {
            return _partnerModuleAppService.CreateCampaignPosts(input);
        }

        [HttpPost]
        [Route("create-group")]
        public Task<GroupDto> CreateGroup(GroupCreateDto groupCreateDto)
        {
            return _partnerModuleAppService.CreateGroup(groupCreateDto);
        }

        [HttpPut]
        [Route("update-group")]
        public Task<GroupDto> UpdateGroupAsync(Guid id, GroupUpdateDto groupUpdateDto)
        {
            return _partnerModuleAppService.UpdateGroupAsync(id, groupUpdateDto);
        }

        [HttpGet]
        [Route("get-growth-campaign-chart-data")]
        public Task<GrowthCampaignChartDto> GetGrowthCampaignChart(GetGrowthCampaignChartsInput input)
        {
            return _partnerModuleAppService.GetGrowthCampaignChart(input);
        }

        [HttpPost]
        [Route("partner-create-post")]
        public Task<PostDto> CreatePost(PostCreateDto input)
        {
            return _partnerModuleAppService.CreatePost(input);
        }


        [HttpPost]
        [Route("partner-create-multiple-posts")]
        public Task CreateMultiplePosts(PostCreateDto input)
        {
            return _partnerModuleAppService.CreateMultiplePosts(input);
        }


        [HttpPut]
        [Route("partner-update-post")]
        public Task<PostDto> UpdatePost(Guid id, PostUpdateDto input)
        {
            return _partnerModuleAppService.UpdatePost(id, input);
        }

        [HttpDelete]
        [Route("delete-post/{id}")]
        public Task DeletePost(Guid id)
        {
            return _partnerModuleAppService.DeletePost(id);
        }

        [HttpGet]
        [Route("get-partners-by-user/{userId}")]
        public Task<List<PartnerDto>> GetPartnersByUser(Guid userId)
        {
            return _partnerModuleAppService.GetPartnersByUser(userId);
        }

        [HttpGet]
        [Route("get-tiktoks")]
        public Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input)
        {
            return _partnerModuleAppService.GetTikToks(input);
        }

        [HttpGet]
        [Route("get-tiktoks-paging")]
        public Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetTikToksPaging(GetTiktoksInputExtend input)
        {
            return _partnerModuleAppService.GetTikToksPaging(input);
        }

        [HttpPut]
        [Route("update-campaign-tiktok")]
        public Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id)
        {
            return _partnerModuleAppService.UpdateCampaignTiktok(input, id);
        }

        [HttpGet]
        [Route("get-partner-users")]
        public Task<List<AppUserDto>> GetPartnerUsers()
        {
            return _partnerModuleAppService.GetPartnerUsers();
        }

        [HttpPost]
        [Route("create-partner-user")]
        public Task AddPartnerUser(string email, string name, string surname)
        {
            return _partnerModuleAppService.AddPartnerUser(email, name, surname);
        }

        [HttpGet]
        [Route("check-user-exist/{email}")]
        public Task<bool> UserExist(string email)
        {
            return _partnerModuleAppService.UserExist(email);
        }

        [HttpDelete]
        [Route("remove-partner-user/{userId}")]
        public Task RemovePartnerUser(Guid userId)
        {
            return _partnerModuleAppService.RemovePartnerUser(userId);
        }

        [HttpGet]
        [Route("get-partner-campaigns-chart")]
        public Task<PartnerCampaignChartDto> GetPartnerCampaignsChart()
        {
            return _partnerModuleAppService.GetPartnerCampaignsChart();
        }

        [HttpGet]
        [Route("get-partner-post-content-type-chart")]
        public Task<PartnerPostTypeChartDto> GetPartnerPostContentTypeChart(DateTime startDate, DateTime endDate)
        {
            return _partnerModuleAppService.GetPartnerPostContentTypeChart(startDate, endDate);
        }

        [HttpGet]
        [Route("get-partner-growth-campaigns-chart")]
        public Task<GrowthCampaignChartDto> GetGrowthCampaignChartsAsync(GetGrowthCampaignChartsInput input)
        {
            return _partnerModuleAppService.GetGrowthCampaignChartsAsync(input);
        }

        [HttpGet]
        [Route("partner-get-post")]
        public Task<PostDto> GetPostAsync(Guid postId)
        {
            return _partnerModuleAppService.GetPostAsync(postId);
        }

        [HttpPut]
        [Route("partner-update-post-note")]
        public Task<PostDto> UpdateNotePost(Guid id, PostUpdateNoteDto input)
        {
            return _partnerModuleAppService.UpdateNotePost(id, input);
        }
    }
}