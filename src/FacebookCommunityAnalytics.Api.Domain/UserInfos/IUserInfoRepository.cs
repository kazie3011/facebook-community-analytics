using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public interface IUserInfoRepository : IRepository<UserInfo, Guid>
    {
        Task<UserInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );
        Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
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
        );

        Task<List<UserInfo>> GetListAsync(
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
                    bool? isGDLStaff = null,
                    bool? isSystemUser = null,
                    bool? isActive = null,
                    bool? enablePayrollCalculation = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            bool? isGDLStaff = null,
            bool? isSystemUser = null,
            bool? isActive = null,
            bool? enablePayrollCalculation = null,
            Guid? appUserId = null,
            CancellationToken cancellationToken = default);

        Task<long> GetCountExtendAsync(
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
            CancellationToken cancellationToken = default);

        Task<UserInfo> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<UserInfoWithNavigationProperties>> Get(params Guid[] userIds);
        Task<long> GetCurrentUserCode(CancellationToken cancellationToken = default);
    }
}