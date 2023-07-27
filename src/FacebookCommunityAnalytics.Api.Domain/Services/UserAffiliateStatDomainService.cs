using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserAffiliateStats;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public class SyncUserAffStatApiRequest
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
    }

    public interface IUserAffiliateStatDomainService : IDomainService
    {
        Task<int> SyncStats(SyncUserAffStatApiRequest apiRequest);
    }

    public class UserAffiliateStatDomainService : DomainService, IUserAffiliateStatDomainService
    {
        private readonly IRepository<UserAffiliateStat, Guid> _userAffiliateStatRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IShopinessDomainService _shopinessDomainService;

        public UserAffiliateStatDomainService(
            IRepository<UserAffiliateStat, Guid> userAffiliateStatRepository,
            IUserDomainService userDomainService,
            IShopinessDomainService shopinessDomainService)
        {
            _userAffiliateStatRepository = userAffiliateStatRepository;
            _userDomainService = userDomainService;
            _shopinessDomainService = shopinessDomainService;
        }

        public async Task<int> SyncStats(SyncUserAffStatApiRequest apiRequest)
        {
            var userDetails = await _userDomainService.GetUserDetails(new ApiUserDetailsRequest()
            {
                GetSystemUsers = false,
                GetActiveUsers = true,
            });

            var userAffiliateStats = await _userAffiliateStatRepository.GetListAsync(x => x.CreatedAt >= apiRequest.FromDateTime.Date);
            foreach (var from in apiRequest.FromDateTime.EachDay(apiRequest.ToDateTime))
            {
                foreach (var user in userDetails.OrderBy(x => x.Info.Code.ToIntODefault()))
                {
                    var current = userAffiliateStats.FirstOrDefault
                    (
                        _ => _.AppUserId == user.User.Id
                             && _.CreatedAt.Date == from
                             && _.AffiliateOwnershipType == apiRequest.AffiliateOwnershipType
                    );
                    if (current != null)
                    {
                        Debug.WriteLine
                        (
                            $"===============DoSyncUserAffStat: ALREADY DONE {apiRequest.AffiliateOwnershipType} "
                            + $"{from:d} to {from.AddDays(1):d} "
                            + $"for {user.Info.Code} - {user.User.UserName}\t\t"
                        );
                        continue;
                    }

                    var stat = await DoSyncUserAffStat
                    (
                        userAffiliateStats,
                        user,
                        from,
                        from.AddDays(1),
                        apiRequest.AffiliateOwnershipType
                    );
                    if (stat.Key != null)
                    {
                        if (stat.Value) { await _userAffiliateStatRepository.InsertAsync(stat.Key); }
                        else { await _userAffiliateStatRepository.UpdateAsync(stat.Key); }
                    }

#if DEBUG
                    var click = stat.Key?.AffConversionModel.ClickCount;
                    var conversion = stat.Key?.AffConversionModel.ConversionCount;

                    Debug.WriteLine
                    (
                        $"===============DoSyncUserAffStat: {apiRequest.AffiliateOwnershipType} "
                        + $"{from:d} to {from.AddDays(1):d} "
                        + $"for {user.Info.Code} - {user.User.UserName}\t\t"
                        + $"| Click:{click} - Conversion:{conversion}"
                    );
#endif

                    await Task.Delay(ShopinessApiConfig.ApiDelayInMs);
                }

                Debug.WriteLine($"===============DoSyncUserAffStat: FINISHED FOR {apiRequest.AffiliateOwnershipType} {from} - {from.AddDays(1)} ");
            }

            return 0;
        }

        private async Task<KeyValuePair<UserAffiliateStat, bool>> DoSyncUserAffStat(
            List<UserAffiliateStat> currentAffs,
            UserDetail user,
            DateTime start,
            DateTime end,
            AffiliateOwnershipType affiliateOwnershipType)
        {
            var isGdl = affiliateOwnershipType == AffiliateOwnershipType.GDL;

            var response = await _shopinessDomainService.GetStat
            (
                new ShopinessAffiliateStatRequest
                {
                    StartDate = start,
                    EndDate = end,
                    IsGDL = isGdl,
                    UserCode = user.Info.Code
                }
            );

            var affiliateStatistic = response.ListData.FirstOrDefault();
            if (affiliateStatistic == null) return new KeyValuePair<UserAffiliateStat, bool>(null, true);

            var current = currentAffs.FirstOrDefault
            (
                _ => _.AppUserId == user.User.Id
                     && _.CreatedAt.Date == start
                     && _.AffiliateOwnershipType == affiliateOwnershipType
            );
            if (current == null)
            {
                var newStat = new UserAffiliateStat
                {
                    AppUserId = user.User.Id,
                    AffiliateOwnershipType = affiliateOwnershipType,
                    CreatedAt = start,
                    AffConversionModel = new AffConversionModel
                    {
                        ClickCount = affiliateStatistic.Click,
                        ConversionCount = affiliateStatistic.Conversion,
                        ConversionAmount = affiliateStatistic.Value,
                        CommissionAmount = affiliateStatistic.Commission,
                        CommissionBonusAmount = affiliateStatistic.CommissionBonus
                    }
                };
                return new KeyValuePair<UserAffiliateStat, bool>(newStat, true);
            }
            else
            {
                current.AffConversionModel.ClickCount = affiliateStatistic.Click;
                current.AffConversionModel.ConversionCount = affiliateStatistic.Conversion;
                current.AffConversionModel.ConversionAmount = affiliateStatistic.Value;
                current.AffConversionModel.CommissionAmount = affiliateStatistic.Commission;
                current.AffConversionModel.CommissionBonusAmount = affiliateStatistic.CommissionBonus;
                return new KeyValuePair<UserAffiliateStat, bool>(current, false);
            }
        }
    }
}