using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public interface ICampaignRepository : IRepository<Campaign, Guid>
    {
        Task<CampaignWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<CampaignWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Campaign>> GetListAsync(
                    string filterText = null,
                    string name = null,
                    string code = null,
                    string hashtags = null,
                    string description = null,
                    CampaignType? campaignType = null,
                    CampaignStatus? status = null,
                    DateTime? startDateTimeMin = null,
                    DateTime? startDateTimeMax = null,
                    DateTime? endDateTimeMin = null,
                    DateTime? endDateTimeMax = null,
                    bool? isActive = null,
                    Guid? partnerId = null,
                    string currentUserEmail = null,
                    List<Guid> partnerIds = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default);

        Task<List<CampaignWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
            string filterText = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountExtendAsync(
            string filterText = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default);
    }
}