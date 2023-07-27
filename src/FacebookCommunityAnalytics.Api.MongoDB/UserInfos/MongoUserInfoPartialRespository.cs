using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public partial class MongoUserInfoRepository
    {
        public async Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
            string filterText = null,
            string code = null,
            string identityNumber = null,
            string facebook = null,
            DateTime? dateOfBirthMin = null,
            DateTime? dateOfBirthMax = null,
            DateTime? joinedDateTimeMin = null,
            DateTime? joinedDateTimeMax = null,
            DateTime? promotedDateTimeMin = null,
            DateTime? promotedDateTimeMax = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            ContentRoleType? contentRole = null,
            UserPosition? userPosition = null,
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        )
        {
            var query = ApplyFilterExtend((await GetMongoQueryableAsync(cancellationToken)), filterText, code, identityNumber,
                facebook, dateOfBirthMin, dateOfBirthMax, joinedDateTimeMin,
                joinedDateTimeMax, promotedDateTimeMin, promotedDateTimeMax, affiliateMultiplierMin,
                affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, contentRole, userPosition, isGDLStaff, isSystemUser, isActive, enablePayrollCalculation, appUserId, appUserIds);
            var userInfos = await query.OrderBy(string.IsNullOrWhiteSpace(sorting)
                    ? UserInfoConsts.GetDefaultSorting(false)
                    : sorting.Split('.').Last())
                .As<IMongoQueryable<UserInfo>>()
                .PageBy<UserInfo, IMongoQueryable<UserInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userInfos.Select
                (
                    s => new UserInfoWithNavigationProperties
                    {
                        UserInfo = s,
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                    }
                )
                .ToList();
        }

        public async Task<long> GetCountExtendAsync(
            string filterText = null,
            string code = null,
            string identityNumber = null,
            string facebook = null,
            DateTime? dateOfBirthMin = null,
            DateTime? dateOfBirthMax = null,
            DateTime? joinedDateTimeMin = null,
            DateTime? joinedDateTimeMax = null,
            DateTime? promotedDateTimeMin = null,
            DateTime? promotedDateTimeMax = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            ContentRoleType? contentRole = null,
            UserPosition? userPosition = null,
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend((await GetMongoQueryableAsync(cancellationToken)), filterText, code, identityNumber,
                facebook, dateOfBirthMin, dateOfBirthMax, joinedDateTimeMin,
                joinedDateTimeMax, promotedDateTimeMin, promotedDateTimeMax, affiliateMultiplierMin,
                affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, contentRole, userPosition, isGDLStaff, isSystemUser, isActive, enablePayrollCalculation, appUserId, appUserIds);
            return await query.As<IMongoQueryable<UserInfo>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCurrentUserCode(CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            return dbContext.UserInfos.AsQueryable().Count();
        }

        protected virtual IQueryable<UserInfo> ApplyFilterExtend(
            IQueryable<UserInfo> query,
            string filterText,
            string code = null,
            string identityNumber = null,
            string facebook = null,
            DateTime? dateOfBirthMin = null,
            DateTime? dateOfBirthMax = null,
            DateTime? joinedDateTimeMin = null,
            DateTime? joinedDateTimeMax = null,
            DateTime? promotedDateTimeMin = null,
            DateTime? promotedDateTimeMax = null,
            double? affiliateMultiplierMin = null,
            double? affiliateMultiplierMax = null,
            double? seedingMultiplierMin = null,
            double? seedingMultiplierMax = null,
            ContentRoleType? contentRoleType = null,
            UserPosition? userPosition = null,
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null)
        {
            query=query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Code.Contains(filterText)
                         || e.IdentityNumber.ToLower()
                             .Contains(filterText.ToLower())
                         || e.Facebook.ToLower()
                             .Contains(filterText.ToLower())
                )
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.ToLower().Contains(code.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber.ToLower().Contains(identityNumber.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(facebook), e => e.Facebook.ToLower().Contains(facebook.ToLower()))
                .WhereIf(dateOfBirthMin.HasValue, e => e.DateOfBirth >= dateOfBirthMin.Value)
                .WhereIf(dateOfBirthMax.HasValue, e => e.DateOfBirth <= dateOfBirthMax.Value)
                .WhereIf(joinedDateTimeMin.HasValue, e => e.JoinedDateTime >= joinedDateTimeMin.Value)
                .WhereIf(joinedDateTimeMax.HasValue, e => e.JoinedDateTime <= joinedDateTimeMax.Value)
                .WhereIf(promotedDateTimeMin.HasValue, e => e.PromotedDateTime >= promotedDateTimeMin.Value)
                .WhereIf(promotedDateTimeMax.HasValue, e => e.PromotedDateTime <= promotedDateTimeMax.Value)
                .WhereIf(affiliateMultiplierMin.HasValue, e => e.AffiliateMultiplier >= affiliateMultiplierMin.Value)
                .WhereIf(affiliateMultiplierMax.HasValue, e => e.AffiliateMultiplier <= affiliateMultiplierMax.Value)
                .WhereIf(seedingMultiplierMin.HasValue, e => e.SeedingMultiplier >= seedingMultiplierMin.Value)
                .WhereIf(seedingMultiplierMax.HasValue, e => e.SeedingMultiplier <= seedingMultiplierMax.Value)
                .WhereIf(contentRoleType.HasValue, e => e.ContentRoleType == contentRoleType)
                .WhereIf(userPosition.HasValue, e => e.UserPosition == userPosition)
                .WhereIf(isGDLStaff.HasValue, e => e.IsGDLStaff == isGDLStaff)
                .WhereIf(isSystemUser.HasValue, e => e.IsSystemUser == isSystemUser)
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                .WhereIf(enablePayrollCalculation.HasValue, e => e.EnablePayrollCalculation == enablePayrollCalculation)
                .WhereIf(appUserId.HasValue && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(appUserIds != null, e => appUserIds.Contains((Guid) e.AppUserId));

            return query;
        }
    }
}