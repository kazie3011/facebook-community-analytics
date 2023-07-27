using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Partners
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Partners.Default)]
    public class PartnersAppService : ApplicationService, IPartnersAppService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IRepository<Contract, Guid> _contractRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;

        public PartnersAppService(
            IPartnerRepository partnerRepository,
            IRepository<Contract, Guid> contractRepository,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager)
        {
            _partnerRepository = partnerRepository;
            _contractRepository = contractRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public virtual async Task<PagedResultDto<PartnerDto>> GetListAsync(GetPartnersInput input)
        {
            if (CurrentUser.IsInRole(RoleConsts.Partner)) input.PartnerUserId = CurrentUser.GetId();
            var totalCount = await _partnerRepository.GetCountAsync
            (
                input.FilterText,
                input.Name,
                input.Description,
                input.Url,
                input.Code,
                input.PartnerType,
                input.IsActive,
                input.PartnerUserId
            );
            var items = await _partnerRepository.GetListAsync
            (
                input.FilterText,
                input.Name,
                input.Description,
                input.Url,
                input.Code,
                input.PartnerType,
                input.IsActive,
                input.PartnerUserId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<PartnerDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Partner>, List<PartnerDto>>(items)
            };
        }

        public virtual async Task<PartnerDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Partner, PartnerDto>(await _partnerRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.Partners.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _partnerRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.Partners.Create)]
        public virtual async Task<PartnerDto> CreateAsync(PartnerCreateDto input)
        {
            if (CurrentUser.Id != null && input.PartnerUserIds.Contains(CurrentUser.Id.Value))
            {
                input.PartnerUserIds.Add(CurrentUser.Id.Value);
            }

            var partner = ObjectMapper.Map<PartnerCreateDto, Partner>(input);
            partner.TenantId = CurrentTenant.Id;
            partner = await _partnerRepository.InsertAsync(partner, autoSave: true);
            return ObjectMapper.Map<Partner, PartnerDto>(partner);
        }

        [Authorize(ApiPermissions.Partners.Edit)]
        public virtual async Task<PartnerDto> UpdateAsync(Guid id, PartnerUpdateDto input)
        {
            if (CurrentUser.Id != null && input.PartnerUserIds.Contains(CurrentUser.Id.Value))
            {
                input.PartnerUserIds.Add(CurrentUser.Id.Value);
            }

            var partner = await _partnerRepository.GetAsync(id);
            ObjectMapper.Map(input, partner);
            partner = await _partnerRepository.UpdateAsync(partner);
            return ObjectMapper.Map<Partner, PartnerDto>(partner);
        }

        public async Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input)
        {
            var userIds = (await _userManager.GetUsersInRoleAsync(RoleConsts.Partner)).Select(x => x.Id).ToList();
            var query = _userRepository.AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData);
        }
    }
}