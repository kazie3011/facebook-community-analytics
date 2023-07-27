using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public interface IUserWaveRepository : IRepository<UserWave, Guid>
    {
        Task<UserWaveWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserWaveWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<UserWave>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}