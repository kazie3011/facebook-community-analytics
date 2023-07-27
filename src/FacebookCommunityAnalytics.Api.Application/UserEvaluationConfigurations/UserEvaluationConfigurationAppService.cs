using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Core;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.BoD.Default)]
    [Authorize(ApiPermissions.UserEvaluationConfigurations.Default)]
    public class UserEvaluationConfigurationAppService : ApplicationService, IUserEvaluationConfigurationAppService
    {
        private readonly IUserEvaluationConfigurationRepository _userEvaluationConfigurationRepository;
        private readonly IdentityUserManager _identityUserManager;

        public UserEvaluationConfigurationAppService(
            IUserEvaluationConfigurationRepository userEvaluationConfigurationRepository,
            IdentityUserManager identityUserManager)
        {
            _userEvaluationConfigurationRepository = userEvaluationConfigurationRepository;
            _identityUserManager = identityUserManager;
        }

        public async Task<UserEvaluationConfigurationDto> GetEvalConfig(Guid appUserId)
        {
            if (appUserId == Guid.Empty) return await CreateOrUpdateRootConfig(null);
            return await GetUserEvalConfig(appUserId);
        }

        public async Task<UserEvaluationConfigurationDto> GetUserEvalConfig(Guid appUserId)
        {
            var userConfig = _userEvaluationConfigurationRepository.FirstOrDefault(x => x.AppUserId.HasValue && x.AppUserId.Value == appUserId);
            if (userConfig != null) return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userConfig);
            var currentUser = await _identityUserManager.GetByIdAsync(appUserId);
            userConfig = new UserEvaluationConfiguration();
            userConfig.OrganizationId = currentUser.OrganizationUnits?.FirstOrDefault()?.OrganizationUnitId;
            userConfig.AppUserId = appUserId;

            await _userEvaluationConfigurationRepository.InsertAsync(userConfig);
            return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userConfig);
        }

        public async Task<UserEvaluationConfigurationDto> UpdateUserEvalConfig(Guid appUserId, UserEvaluationConfigurationDto input)
        {
            var currentUser = await _identityUserManager.GetByIdAsync(appUserId);
            var OrganizationId = currentUser.OrganizationUnits?.FirstOrDefault()?.OrganizationUnitId;
            var oldConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.OrganizationId == OrganizationId && x.AppUserId == appUserId);
            var updateConfig = ObjectMapper.Map(input, oldConfig);
            await _userEvaluationConfigurationRepository.UpdateAsync(updateConfig);
            return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(updateConfig);
        }

        public async Task<UserEvaluationConfigurationDto> CreateAsync(UserEvaluationConfigurationCreateUpdateDto input)
        {
            var userEvaluationConfig = ObjectMapper.Map<UserEvaluationConfigurationCreateUpdateDto, UserEvaluationConfiguration>(input);

            var currentUser = await _identityUserManager.GetByIdAsync(CurrentUser.Id.GetValueOrDefault());
            userEvaluationConfig.OrganizationId = currentUser.OrganizationUnits?.FirstOrDefault()?.OrganizationUnitId;

            await _userEvaluationConfigurationRepository.InsertAsync(userEvaluationConfig);
            return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        }

        public async Task<UserEvaluationConfigurationDto> CreateOrUpdateCustomConfig(UserEvaluationConfigurationDto input)
        {
            if (input == null) throw new ArgumentNullException("CreateOrUpdateCustomConfig.input");

            // valid data, can create custom config
            UserEvaluationConfiguration userCustomConfig = null;
            if (input.TeamId != null && input.TeamId != Guid.Empty)
            {
                if (input.AppUserId.IsNotNullOrEmpty())
                {
                    userCustomConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.TeamId == input.TeamId && x.AppUserId == input.AppUserId);
                    if (userCustomConfig == null)
                    {
                        userCustomConfig = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfiguration>(input);
                        await _userEvaluationConfigurationRepository.InsertAsync(userCustomConfig);
                    }
                }
                else
                {
                    userCustomConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.TeamId == input.TeamId && x.UserPosition == input.UserPosition);
                    if (userCustomConfig == null)
                    {
                        userCustomConfig = ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfiguration>(input);
                        await _userEvaluationConfigurationRepository.InsertAsync(userCustomConfig);
                    }
                }
            }

            return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userCustomConfig);
        }

        public async Task<UserEvaluationConfigurationDto> CreateOrUpdateRootConfig(UserEvaluationConfigurationDto input)
        {
            // create root config if not existing
            var rootConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.OrganizationId == null && x.AppUserId == null && x.TeamId == null && x.UserPosition == null);
            if (rootConfig == null)
            {
                rootConfig = input != null ? ObjectMapper.Map<UserEvaluationConfigurationDto, UserEvaluationConfiguration>(input) : new UserEvaluationConfiguration();
                rootConfig = await _userEvaluationConfigurationRepository.InsertAsync(rootConfig);

                return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(rootConfig);
            }
            else
            {
                if (input != null)
                {
                    var rootConfigToUpdate = ObjectMapper.Map(input, rootConfig);
                    await _userEvaluationConfigurationRepository.UpdateAsync(rootConfigToUpdate);
                    return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(rootConfigToUpdate);
                }
                else
                {
                    return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(rootConfig);
                }
            }
        }

        public async Task<List<UserEvaluationConfigurationDto>> GetUserEvaluationConfigurations(GetUserEvaluationConfigurationsInput request)
        {
            if (request.TeamId.IsNullOrEmpty() && request.TeamIds.IsNullOrEmpty()) return new List<UserEvaluationConfigurationDto>();
            var userConfigs = new List<UserEvaluationConfiguration>();
            if (request.TeamId.IsNotNullOrEmpty()) userConfigs = await _userEvaluationConfigurationRepository.GetListAsync(x => x.TeamId == request.TeamId);
            if (request.TeamIds.IsNotNullOrEmpty()) userConfigs = await _userEvaluationConfigurationRepository.GetListAsync(x => x.TeamId != null && request.TeamIds.Contains(x.TeamId.Value) );
            
            return ObjectMapper.Map<List<UserEvaluationConfiguration>, List<UserEvaluationConfigurationDto>>(userConfigs);
        }

        public async Task UpdateAsync(Guid id, UserEvaluationConfigurationCreateUpdateDto input)
        {
            var userEvaluationConfiguration = await _userEvaluationConfigurationRepository.GetAsync(id);
            ObjectMapper.Map(input, userEvaluationConfiguration);
            await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfiguration);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _userEvaluationConfigurationRepository.DeleteAsync(id);
        }


        // [Authorize(ApiPermissions.UserEvaluationConfigurations.ConfigSale)]
        // public async Task<UserEvaluationConfigurationDto> UpdateSaleEvaluationConfigurationAsync(Guid id, SaleEvaluationConfigurationDto input)
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.Id == id);
        //     if (userEvaluationConfig == null)
        //     {
        //         return null;
        //     }
        //
        //     var saleEvaluationConfig = ObjectMapper.Map<SaleEvaluationConfigurationDto, SaleEvaluationConfiguration>(input);
        //     userEvaluationConfig.Sale = saleEvaluationConfig;
        //
        //     await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfig);
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
        //
        // [Authorize(ApiPermissions.UserEvaluationConfigurations.ConfigTiktok)]
        // public async Task<UserEvaluationConfigurationDto> UpdateTiktokEvaluationConfigurationAsync(Guid id, TiktokEvaluationConfigurationDto input)
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.Id == id);
        //     if (userEvaluationConfig == null)
        //     {
        //         return null;
        //     }
        //
        //     var tiktokEvaluationConfig = ObjectMapper.Map<TiktokEvaluationConfigurationDto, TiktokEvaluationConfiguration>(input);
        //     userEvaluationConfig.Tiktok = tiktokEvaluationConfig;
        //
        //     await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfig);
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
        //
        // [Authorize(ApiPermissions.UserEvaluationConfigurations.ConfigContent)]
        // public async Task<UserEvaluationConfigurationDto> UpdateContentEvaluationConfigurationAsync(Guid id, ContentEvaluationConfigurationDto input)
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.Id == id);
        //     if (userEvaluationConfig == null)
        //     {
        //         return null;
        //     }
        //
        //     var contentEvaluationConfig = ObjectMapper.Map<ContentEvaluationConfigurationDto, ContentEvaluationConfiguration>(input);
        //     userEvaluationConfig.Content = contentEvaluationConfig;
        //
        //     await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfig);
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
        //
        // [Authorize(ApiPermissions.UserEvaluationConfigurations.ConfigSeeding)]
        // public async Task<UserEvaluationConfigurationDto> UpdateSeedingEvaluationConfigurationAsync(Guid id, SeedingEvaluationConfigurationDto input)
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.Id == id);
        //     if (userEvaluationConfig == null)
        //     {
        //         return null;
        //     }
        //
        //     var seedingEvaluationConfig = ObjectMapper.Map<SeedingEvaluationConfigurationDto, SeedingEvaluationConfiguration>(input);
        //     userEvaluationConfig.Seeding = seedingEvaluationConfig;
        //
        //     await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfig);
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
        //
        // [Authorize(ApiPermissions.UserEvaluationConfigurations.ConfigAffiliate)]
        // public async Task<UserEvaluationConfigurationDto> UpdateAffiliateEvaluationConfigurationAsync(Guid id, AffiliateEvaluationConfigurationDto input)
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.FindAsync(x => x.Id == id);
        //     if (userEvaluationConfig == null)
        //     {
        //         return null;
        //     }
        //
        //     var affiliateEvaluationConfig = ObjectMapper.Map<AffiliateEvaluationConfigurationDto, AffiliateEvaluationConfiguration>(input);
        //     userEvaluationConfig.Affiliate = affiliateEvaluationConfig;
        //
        //     await _userEvaluationConfigurationRepository.UpdateAsync(userEvaluationConfig);
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
        //
        // [Authorize(ApiPermissions.UserEvaluationConfigurations.Default)]
        // public async Task<UserEvaluationConfigurationDto> GetTeamEvaluationConfigForCurrentUser()
        // {
        //     var userEvaluationConfig = await _userEvaluationConfigurationRepository.GetByUserId(CurrentUser.Id.GetValueOrDefault());
        //     return ObjectMapper.Map<UserEvaluationConfiguration, UserEvaluationConfigurationDto>(userEvaluationConfig);
        // }
    }
}