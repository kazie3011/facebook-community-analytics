using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.Users;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using UrlHelper = FacebookCommunityAnalytics.Api.Core.Helpers.UrlHelper;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserCompensations.Default)]
    public class UserCompensationAppService : BaseCrudApiAppService<UserCompensation, UserCompensationDto, Guid, GetUserCompensationsInput, CreateUpdateUserCompensationDto>,
        IUserCompensationAppService
    {
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly IUserCompensationRepository _userCompensationRepository;
        private readonly IUserCompensationDomainService _userCompensationDomainService;
        private readonly IExportDomainService _exportDomainService;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IPostRepository _postRepository;
        private readonly IDistributedEventBus _distributedEventBus;

        public UserCompensationAppService(
            IRepository<UserCompensation, Guid> repository,
            IRepository<AppUser, Guid> appUserRepository,
            IUserCompensationRepository userCompensationRepository,
            IUserCompensationDomainService userCompensationDomainService,
            IExportDomainService exportDomainService,
            IPayrollRepository payrollRepository,
            IAffiliateConversionRepository affiliateConversionRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IPostRepository postRepository,
            IDistributedEventBus distributedEventBus) : base(repository)
        {
            _appUserRepository = appUserRepository;
            _userCompensationRepository = userCompensationRepository;
            _userCompensationDomainService = userCompensationDomainService;
            _exportDomainService = exportDomainService;
            _payrollRepository = payrollRepository;
            _affiliateConversionRepository = affiliateConversionRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _postRepository = postRepository;
            _distributedEventBus = distributedEventBus;
        }

        public override async Task<PagedResultDto<UserCompensationDto>> GetListAsync(GetUserCompensationsInput input)
        {
            var userCompensations = await _userCompensationRepository.GetListAsync
            (
                input.FilterText,
                input.Month,
                input.Year,
                input.PayrollId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var count = await _userCompensationRepository.GetLongCountAsync
            (
                input.FilterText,
                input.Month,
                input.Year
            );

            return new PagedResultDto<UserCompensationDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<UserCompensation>, List<UserCompensationDto>>(userCompensations)
            };
        }

        public async Task<PagedResultDto<UserCompensationNavigationPropertiesDto>> GetListWithNavigationAsync(GetUserCompensationsInput input)
        {
            var userCompensations = await _userCompensationRepository.GetListWithNavigationPropertiesAsync
            (
                input.FilterText,
                input.Month,
                input.Year,
                input.PayrollId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var count = await _userCompensationRepository.GetLongCountAsync
            (
                input.FilterText,
                input.Month,
                input.Year
            );

            return new PagedResultDto<UserCompensationNavigationPropertiesDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<UserCompensationNavigationProperties>, List<UserCompensationNavigationPropertiesDto>>(userCompensations)
            };
        }

        public async Task<UserCompensationNavigationPropertiesDto> GetWithNavigationAsync(Guid id)
        {
            var userCompensation = await _userCompensationRepository.GetWithNavigationPropertiesAsync(id);
            return ObjectMapper.Map<UserCompensationNavigationProperties, UserCompensationNavigationPropertiesDto>(userCompensation);
        }

        public async Task<UserCompensationNavigationPropertiesDto> GetWithNavigationByUserAsync(Guid userId, int month, int year)
        {
            var userCompensation = await _userCompensationRepository.GetWithNavigationPropertiesByUserAsync(userId, month, year);
            
            return ObjectMapper.Map<UserCompensationNavigationProperties, UserCompensationNavigationPropertiesDto>(userCompensation);
        }

        public async Task CalculateCompensation(int month, int year,bool isHappyDay = false)
        {
            Hangfire.BackgroundJob.Enqueue
            (
                () => _userCompensationDomainService.CalculateCompensations
                (
                    true,
                    true,
                    isHappyDay,
                    CurrentUser.Id,
                    month,
                    year
                )
            );
            if (CurrentUser.Id.HasValue)
            {
                await _distributedEventBus.PublishAsync(new ReceivedMessageEto(CurrentUser.Id.Value, CurrentUser.UserName, L["Message.DoneCalculateCompensation"]));
            }
        }

        public async Task<byte[]> ExportCompensation(Guid payrollId)
        {
            var payroll = await _payrollRepository.GetAsync(payrollId);
            if (payroll == null)
            {
                throw new BusinessException("Payroll not exits.");
            }

            var userCompensationsWithNav = await _userCompensationRepository.GetListWithNavigationPropertiesAsync
            (
                payrollId: payrollId
            );
            var userCompensationExportRows = userCompensationsWithNav.Select
                (
                    x =>
                    {
                        var bonusDetail = x.UserCompensation.Bonuses.Aggregate(string.Empty, (current, item) => current + ($"- {L[$"Enum:BonusType:{Convert.ToInt32(item.BonusType ?? BonusType.Unknown)}"] + ": " + item.BonusAmount.ToVND()} "));
                        var finesDetail = x.UserCompensation.Bonuses.Where(_ => _.FinesDescription.IsNotNullOrEmpty()).Aggregate(string.Empty, (current, item) => current + ($"- {item.FinesDescription} : {item.FinesAmount.ToVND()}"));
                        var bonusAmount = x.UserCompensation.Bonuses.Sum(s => s.BonusAmount);
                        var finesAmount = x.UserCompensation.Bonuses.Sum(s => s.FinesAmount);
                        var salary = x.UserCompensation.SalaryAmount;
                        var totalAmount = x.UserCompensation.TotalAmount;
                        var description = string.Empty;                                          
                        if (x.UserCompensation.Description != null && x.UserCompensation.Description.Contains(("|")))
                        {
                            description = x.UserCompensation.Description.Split('|').Aggregate(description, (current, part) => current + $"- {part} ");
                        }
                        else
                        {
                            description = x.UserCompensation.Description;
                        }
                        var row = new UserCompensationExportRow
                        {
                            Team = x.UserCompensation.Team,
                            Email = x.AppUser.Email,
                            Username = x.AppUser.UserName,
                            Fullname = $"{x.AppUser.Surname} {x.AppUser.Name}",
                            Position = L[$"Enum:UserPosition:{(Convert.ToInt32(x.UserInfo.UserPosition) == 0 ? 1 : Convert.ToInt32(x.UserInfo.UserPosition))}"],
                            BonusAmount = bonusAmount == 0? string.Empty : bonusAmount.ToVND(),
                            FinesAmount = finesAmount == 0? string.Empty : finesAmount.ToVND(),
                            Salary = salary == 0? string.Empty : salary.ToVND(),
                            BonusDetail = bonusDetail.Trim('-').Trim(),
                            FinesDetail = finesDetail,
                            Description = description?.Trim('-').Trim(),
                            TotalAmount = totalAmount == 0? string.Empty : totalAmount.ToVND()
                        };
                        return row;
                    }
                )
                .ToList();
            // var title = L["PayrollEmail.Title"];
            // var sheetName = $"{title} " + $"{payroll.FromDateTime:GlobalConsts.DateFormat} - " + $"{payroll.ToDateTime:GlobalConsts.DateFormat}";

            userCompensationExportRows = userCompensationExportRows.OrderBy(x => x.Team).ThenBy(u => u.Username).ToList();
            return _exportDomainService.GenerateUserCompensationsExcelBytes(userCompensationExportRows);
        }

        public async Task<CompensationDetailDto> GetCompensationDetailAsync(Guid payrollId)
        {
            var payroll = await _payrollRepository.GetAsync(payrollId);
            var compensationsWithNav = await _userCompensationRepository.GetListWithNavigationPropertiesAsync(payrollId: payrollId);
            var result = new CompensationDetailDto()
            {
                Payroll = ObjectMapper.Map<Payroll, PayrollDto>(payroll),
                TotalStaff = compensationsWithNav.Count,
                TotalAmount = compensationsWithNav.Select(x => x.UserCompensation).Sum(t => t.TotalAmount),
                BonusAmount = compensationsWithNav.Select(x => x.UserCompensation).Sum(s => s.Bonuses.Sum(t => t.BonusAmount)),
                FinesAmount = compensationsWithNav.Select(x => x.UserCompensation).Sum(s => s.Bonuses.Sum(t => t.FinesAmount)),
                IsHappyDay = payroll.Code?.ToLower().Contains(PayrollConsts.HappyDay.ToLower()) ?? false
            };
            foreach (var g in compensationsWithNav.GroupBy(x => x.UserCompensation.Team))
            {
                var list = g.ToList();
                var totalAmount = list.Sum(a => a.UserCompensation.TotalAmount);
                var totalBonus = list.Sum(a => a.UserCompensation.Bonuses.Sum(t => t.BonusAmount));
                var totalFines = list.Sum(a => a.UserCompensation.Bonuses.Sum(t => t.FinesAmount));
                var totalSalary = totalAmount - totalBonus + totalFines ;
                result.TeamCompensations.Add
                (
                    new TeamCompensationDto
                    {
                        Team = g.Key,
                        TotalAmount = totalAmount,
                        TotalSalary = totalSalary,
                        BonusAmount = totalBonus,
                        FinesAmount = totalFines,
                        UserCompensations = ObjectMapper.Map<List<UserCompensationNavigationProperties>, List<UserCompensationNavigationPropertiesDto>>(list)
                    }
                );
            }

            result.TeamCompensations = result.TeamCompensations.OrderBy(x => x.Team).ToList();
            
            return result;
        }

        public async Task ConfirmPayroll(Guid compensation)
        {
            await _userCompensationDomainService.ConfirmPayroll(compensation);
        }

        public async Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId)
        {
            return await _userCompensationDomainService.GetAffiliateConversions(fromDate, toDate, userId);
        }
    }
}