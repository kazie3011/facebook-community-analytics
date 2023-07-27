using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public partial class MongoUserInfoRepository : MongoDbRepositoryBase<ApiMongoDbContext, UserInfo, Guid>,
        IUserInfoRepository
    {
        public MongoUserInfoRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userInfo = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userInfo.AppUserId, cancellationToken: cancellationToken);

            return new UserInfoWithNavigationProperties
            {
                UserInfo = userInfo,
                AppUser = appUser,

            };
        }

        public async Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            ContentRoleType? contentRoleType = null,
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, identityNumber, facebook, dateOfBirthMin, dateOfBirthMax, joinedDateTimeMin, joinedDateTimeMax, promotedDateTimeMin, promotedDateTimeMax, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, contentRoleType, isGDLStaff, isSystemUser, isActive, enablePayrollCalculation, appUserId);
            var userInfos = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserInfoConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserInfo>>()
                .PageBy<UserInfo, IMongoQueryable<UserInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userInfos.Select(s => new UserInfoWithNavigationProperties
            {
                UserInfo = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
            }).ToList();
        }

        public async Task<List<UserInfo>> GetListAsync(
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
            ContentRoleType? contentRoleType = null,
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, identityNumber, facebook, dateOfBirthMin, dateOfBirthMax, joinedDateTimeMin, joinedDateTimeMax, promotedDateTimeMin, promotedDateTimeMax, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, contentRoleType, isGDLStaff, isSystemUser, isActive, enablePayrollCalculation);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserInfoConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserInfo>>()
                .PageBy<UserInfo, IMongoQueryable<UserInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
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
           ContentRoleType? contentRoleType = null,
           bool? isGDLStaff = null,
           bool? isSystemUser = null,
           bool? isActive = null,
           bool? enablePayrollCalculation = null,
           Guid? appUserId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, identityNumber, facebook, dateOfBirthMin, dateOfBirthMax, joinedDateTimeMin, joinedDateTimeMax, promotedDateTimeMin, promotedDateTimeMax, affiliateMultiplierMin, affiliateMultiplierMax, seedingMultiplierMin, seedingMultiplierMax, contentRoleType, isGDLStaff, isSystemUser, isActive, enablePayrollCalculation, appUserId);
            return await query.As<IMongoQueryable<UserInfo>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserInfo> ApplyFilter(
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
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code.ToLower().Contains(filterText.ToLower())
                                                                      || e.IdentityNumber.ToLower()
                                                                          .Contains(filterText.ToLower())
                                                                      || e.Facebook.ToLower()
                                                                          .Contains(filterText.ToLower())
                )
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber.Contains(identityNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(facebook), e => e.Facebook.Contains(facebook))
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
                    .WhereIf(isGDLStaff.HasValue, e => e.IsGDLStaff == isGDLStaff)
                    .WhereIf(isSystemUser.HasValue, e => e.IsSystemUser == isSystemUser)
                    .WhereIf(isSystemUser.HasValue, e => e.IsActive == isActive)
                    .WhereIf(enablePayrollCalculation.HasValue, e => e.EnablePayrollCalculation == enablePayrollCalculation)
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId);
        }

        public async Task<UserInfo> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = await GetMongoQueryableAsync(cancellationToken);
            return query.WhereIf(userId != default, x =>  x.AppUserId == userId)
                .FirstOrDefault();
        }

        public async Task<List<UserInfoWithNavigationProperties>> Get(params Guid[] userIds)
        {
            var query = await GetMongoQueryableAsync();
            var userInfos = query
                .WhereIf(userIds != null, x => x.AppUserId.HasValue && userIds.Contains(x.AppUserId.Value)).ToList();

            var dbContext = await GetDbContextAsync();
            return userInfos.Select(userInfo => new UserInfoWithNavigationProperties
            {
                UserInfo = userInfo,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == userInfo.AppUserId && !e.IsDeleted),
            }).ToList();
        }
    }
}