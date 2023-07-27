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

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class MongoUserWaveRepository : MongoDbRepository<ApiMongoDbContext, UserWave, Guid>, IUserWaveRepository
    {
        public MongoUserWaveRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserWaveWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userWave = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userWave.AppUserId, cancellationToken: cancellationToken);
            var payroll = await (await GetDbContextAsync(cancellationToken)).Payrolls.AsQueryable().FirstOrDefaultAsync(e => e.Id == userWave.PayrollId, cancellationToken: cancellationToken);

            return new UserWaveWithNavigationProperties
            {
                UserWave = userWave,
                AppUser = appUser,
                Payroll = payroll,

            };
        }

        public async Task<List<UserWaveWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            WaveType? waveType = null,
            int? totalPostCountMin = null,
            int? totalPostCountMax = null,
            int? totalReactionCountMin = null,
            int? totalReactionCountMax = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            Guid? appUserId = null,
            Guid? payrollId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, waveType, totalPostCountMin, totalPostCountMax, totalReactionCountMin, totalReactionCountMax, likeCountMin, likeCountMax, commentCountMin, commentCountMax, shareCountMin, shareCountMax, amountMin, amountMax, description, appUserId, payrollId);
            var userWaves = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserWaveConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserWave>>()
                .PageBy<UserWave, IMongoQueryable<UserWave>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userWaves.Select(s => new UserWaveWithNavigationProperties
            {
                UserWave = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId),
                Payroll = dbContext.Payrolls.AsQueryable().FirstOrDefault(e => e.Id == s.PayrollId),

            }).ToList();
        }

        public async Task<List<UserWave>> GetListAsync(
            string filterText = null,
            WaveType? waveType = null,
            int? totalPostCountMin = null,
            int? totalPostCountMax = null,
            int? totalReactionCountMin = null,
            int? totalReactionCountMax = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, waveType, totalPostCountMin, totalPostCountMax, totalReactionCountMin, totalReactionCountMax, likeCountMin, likeCountMax, commentCountMin, commentCountMax, shareCountMin, shareCountMax, amountMin, amountMax, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserWaveConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserWave>>()
                .PageBy<UserWave, IMongoQueryable<UserWave>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           WaveType? waveType = null,
           int? totalPostCountMin = null,
           int? totalPostCountMax = null,
           int? totalReactionCountMin = null,
           int? totalReactionCountMax = null,
           int? likeCountMin = null,
           int? likeCountMax = null,
           int? commentCountMin = null,
           int? commentCountMax = null,
           int? shareCountMin = null,
           int? shareCountMax = null,
           decimal? amountMin = null,
           decimal? amountMax = null,
           string description = null,
           Guid? appUserId = null,
           Guid? payrollId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, waveType, totalPostCountMin, totalPostCountMax, totalReactionCountMin, totalReactionCountMax, likeCountMin, likeCountMax, commentCountMin, commentCountMax, shareCountMin, shareCountMax, amountMin, amountMax, description, appUserId, payrollId);
            return await query.As<IMongoQueryable<UserWave>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserWave> ApplyFilter(
            IQueryable<UserWave> query,
            string filterText,
            WaveType? waveType = null,
            int? totalPostCountMin = null,
            int? totalPostCountMax = null,
            int? totalReactionCountMin = null,
            int? totalReactionCountMax = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string description = null,
            Guid? appUserId = null,
            Guid? payrollId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description.Contains(filterText))
                    .WhereIf(waveType.HasValue, e => e.WaveType == waveType)
                    .WhereIf(totalPostCountMin.HasValue, e => e.TotalPostCount >= totalPostCountMin.Value)
                    .WhereIf(totalPostCountMax.HasValue, e => e.TotalPostCount <= totalPostCountMax.Value)
                    .WhereIf(totalReactionCountMin.HasValue, e => e.TotalReactionCount >= totalReactionCountMin.Value)
                    .WhereIf(totalReactionCountMax.HasValue, e => e.TotalReactionCount <= totalReactionCountMax.Value)
                    .WhereIf(likeCountMin.HasValue, e => e.LikeCount >= likeCountMin.Value)
                    .WhereIf(likeCountMax.HasValue, e => e.LikeCount <= likeCountMax.Value)
                    .WhereIf(commentCountMin.HasValue, e => e.CommentCount >= commentCountMin.Value)
                    .WhereIf(commentCountMax.HasValue, e => e.CommentCount <= commentCountMax.Value)
                    .WhereIf(shareCountMin.HasValue, e => e.ShareCount >= shareCountMin.Value)
                    .WhereIf(shareCountMax.HasValue, e => e.ShareCount <= shareCountMax.Value)
                    .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                    .WhereIf(payrollId != null && payrollId != Guid.Empty, e => e.PayrollId == payrollId);
        }
    }
}