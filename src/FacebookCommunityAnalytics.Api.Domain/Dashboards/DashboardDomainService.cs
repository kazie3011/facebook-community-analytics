using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    public interface IDashboardDomainService : IDomainService
    {
        Task<SaleChartDto> GetSaleChart(SaleChartApiRequest request);
        Task<SaleChartDto> GetContentChart(ContentChartApiRequest request);
        Task<List<DashboardUserDto>> GetSalePerson();
    }
    public class DashboardDomainService : BaseDomainService, IDashboardDomainService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;

        public DashboardDomainService(IContractRepository contractRepository,
            IPostRepository postRepository,
            IUserDomainService userDomainService,
            IOrganizationUnitRepository organizationUnitRepository)
        {
            _contractRepository = contractRepository;
            _postRepository = postRepository;
            _userDomainService = userDomainService;
            _organizationUnitRepository = organizationUnitRepository;
        }

        public async Task<SaleChartDto> GetSaleChart(SaleChartApiRequest request)
        {
            var chart = new SaleChartDto();
            var contractNavs = await _contractRepository.GetListWithNavigationPropertiesAsync(salePersonId: request.SalePersonId, signedAtMin: request.FromDateTime, signedAtMax: request.ToDateTime);
            var contracts = contractNavs.Select(_ => _.Contract).ToList();
            chart.Contracts.AddRange(ObjectMapper.Map<List<ContractWithNavigationProperties>, List<ContractWithNavigationPropertiesDto>>(contractNavs));

            decimal total = 0;
            decimal partial = 0;
            decimal remain = 0;
            foreach (var dateTime in request.FromDateTime.Value.To(request.ToDateTime.Value))
            {
                var dayContracts = contracts.Where(_ => _.SignedAt.HasValue && _.SignedAt.Value.Date == dateTime.Date).ToList();
                
                var totalValue = dayContracts?.Sum(_ => _.TotalValue) == 0? null : dayContracts?.Sum(_ => _.TotalValue);
                total = total + totalValue ?? total;
                chart.TotalBarCharts.Add(new DataChartItemDto<decimal?, string>()
                {
                    Display = dateTime.ToString("dd-MM"),
                    Type = "SaleTotal",
                    Value = total == 0? null : total,
                });

                var partialPaymentValue = dayContracts?.Sum(_ => _.PartialPaymentValue) == 0 ? null : dayContracts?.Sum(_ => _.PartialPaymentValue);
                partial = partial + partialPaymentValue ?? partial;
                chart.PartialPaymentBarCharts.Add(new DataChartItemDto<decimal?, string>()
                {
                    Display = dateTime.ToString("dd-MM"),
                    Type = "SalePartialPayment",
                    Value = partial == 0? null : partial,
                });

                var remainingPaymentValue = dayContracts?.Sum(_ => _.RemainingPaymentValue) == 0 ? null : dayContracts?.Sum(_ => _.RemainingPaymentValue);
                remain = remain + remainingPaymentValue ?? remain;
                chart.RemainingPaymentBarCharts.Add(new DataChartItemDto<decimal?, string>()
                {
                    Display = dateTime.ToString("dd-MM"),
                    Type = "SaleRemainingPayment",
                    Value = remain == 0? null : remain
                });
            }

            return chart;
        }

        public async Task<SaleChartDto> GetContentChart(ContentChartApiRequest request)
        {
            var chart = new SaleChartDto();
            var postsNav = await _postRepository.GetListWithNavigationPropertiesExtendAsync(appUserId: request.ContentPersonId);
            var posts = await _postRepository.GetListExtendAsync(appUserId: request.ContentPersonId);
            foreach (var dateTime in request.FromDateTime.Value.To(request.ToDateTime.Value))
            {
                var dayPosts = posts.Where(_ => _.CreatedDateTime.HasValue && _.CreatedDateTime.Value.Date == dateTime.Date).ToList();
                
                var totalCount = dayPosts?.Sum(_ => _.TotalCount) == 0? null : dayPosts?.Sum(_ => _.TotalCount);
                chart.TotalBarCharts.Add(new DataChartItemDto<decimal?, string>()
                {
                    Display = dateTime.ToString("dd-MM"),
                    Type = "SaleTotal",
                    Value = totalCount == 0? null : totalCount,
                });
            }

            return chart;
        }

        public async Task<List<DashboardUserDto>> GetSalePerson()
        {
            var teamMembers = new List<DashboardUserDto>();
            
            var orgUnits= await _organizationUnitRepository.GetListAsync();
            var saleOrgUnitId = orgUnits.FirstOrDefault(_ => _.DisplayName == TeamMemberConsts.Sale)?.Id;
            if (saleOrgUnitId.IsNullOrEmpty()) return teamMembers;
            
            var userDetails = await _userDomainService.GetUserDetails(new ApiUserDetailsRequest()
            {
                GetTeamUsers = true,
                GetSystemUsers = false,
                GetActiveUsers = true,
                TeamId = saleOrgUnitId
            });
            foreach (var user in userDetails)
            {
                var member = ObjectMapper.Map<UserDetail, DashboardUserDto>(user);
                member.UserCode = user.Info.Code;
                teamMembers.Add(member);
            }

            return teamMembers;
        }
    }
}