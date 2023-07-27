using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IDailyKPITrackingDomainService : IDomainService
    {
        Task<List<UnqualifiedDailyKPIResult>> GetUnqualifiedDailyKPIResults();
    }

    public class DailyKPITrackingDomainService : BaseDomainService, IDailyKPITrackingDomainService
    {
        private readonly IRepository<OrganizationUnit> _organizationUnitsRepository;
        private readonly IUserEvaluationConfigurationRepository _userEvaluationConfigurationRepository;
        private readonly IPostRepository _postRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _identityRoleManager;

        public DailyKPITrackingDomainService(
            IRepository<OrganizationUnit> organizationUnitsRepository,
            IdentityUserManager userManager,
            IdentityRoleManager identityRoleManager,
            IPostRepository postRepository,
            IUserEvaluationConfigurationRepository userEvaluationConfigurationRepository)
        {
            _organizationUnitsRepository = organizationUnitsRepository;
            _userManager = userManager;
            _identityRoleManager = identityRoleManager;
            _postRepository = postRepository;
            _userEvaluationConfigurationRepository = userEvaluationConfigurationRepository;
        }

        public async Task<List<UnqualifiedDailyKPIResult>> GetUnqualifiedDailyKPIResults()
        {
            try
            {
                var results = new List<UnqualifiedDailyKPIResult>();
                var teams = await GetDailyEvaluationTeams();
                if (teams.Any())
                {
                    var yesterday = DateTime.UtcNow.Date.AddDays(-1);
                    foreach (var (team, leaders, users) in teams)
                    {
                        var result = new UnqualifiedDailyKPIResult
                        {
                            Leaders = leaders,
                            UnqualifiedResults = new List<DailyKPIResult>()
                        };
                        var userIds = users.Select(x => x.Id);
                        var organizationIds = users.Where(x => x.OrganizationUnits.Any())
                                                   .Select(x => x.OrganizationUnits.FirstOrDefault().OrganizationUnitId);
                        
                        var posts = await _postRepository.GetListAsync
                        (
                            x => x.AppUserId != null
                                 && userIds.Contains(x.AppUserId.Value)
                                 && x.CreatedDateTime != null
                                 && x.CreatedDateTime >= yesterday
                                 && x.CreatedDateTime < yesterday.AddDays(1)
                        );

                        var userEvaluationConfigs = await _userEvaluationConfigurationRepository.GetListAsync
                        (
                            evalConfig =>
                                   (evalConfig.AppUserId != null && userIds.Contains(evalConfig.AppUserId.Value))
                                || (evalConfig.OrganizationId != null && organizationIds.Contains(evalConfig.OrganizationId.Value))
                                || (evalConfig.OrganizationId == null && evalConfig.AppUserId == null)
                        );

                        var defaultEvalConfig = userEvaluationConfigs.FirstOrDefault
                        (
                            evalConfig => evalConfig.OrganizationId == team.Id && evalConfig.AppUserId == null
                        );


                        var teamEvalConfig = userEvaluationConfigs.FirstOrDefault
                        (
                            evalConfig => evalConfig.OrganizationId == team.Id && evalConfig.AppUserId == null
                        );

                        foreach (var user in users)
                        {
                            var userEvaluationConfig = userEvaluationConfigs.FirstOrDefault
                            (
                                x => x.AppUserId == user.Id
                                     && x.OrganizationId == team.Id
                            );

                            var evalConfig = userEvaluationConfig ?? teamEvalConfig ?? defaultEvalConfig;
                            var userPosts = posts.Where(x => x.AppUserId == user.Id);
                            var teamType = GetTeamType(team.DisplayName, GlobalConfiguration.TeamTypeMapping);
                            var trackingKpiResult = TrackUnqualifiedDailyKPI(user, evalConfig, userPosts, teamType);
                            if (trackingKpiResult != null)
                            {
                                result.UnqualifiedResults.Add(trackingKpiResult);
                            }
                        }

                        if (result.UnqualifiedResults.Any())
                        {
                            results.Add(result);
                        }
                    }
                }
                
                return results;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private DailyKPIResult TrackUnqualifiedDailyKPI(IdentityUser user, UserEvaluationConfiguration evalConfig, IEnumerable<Post> posts, TeamType teamType)
        {
            if (evalConfig == null) return null;
            
            int? requiredQuantity = 0;

            switch (teamType)
            {
                case TeamType.Content:
                    requiredQuantity = evalConfig.Content.ContentPostQuantity;
                    break;
                case TeamType.Affiliate:
                    requiredQuantity = evalConfig.Affiliate.AffiliatePostQuantity;
                    break;
                case TeamType.Seeding:
                    requiredQuantity = evalConfig.Seeding.SeedingPostQuantity;
                    break;
            }

            if (posts.Count() < requiredQuantity.GetValueOrDefault())
            {
                return new DailyKPIResult
                {
                    User = user,
                    RequiredQuantity = requiredQuantity.GetValueOrDefault(),
                    ActualQuantity = posts.Count()
                };
            }

            return null;
        }

        private TeamType GetTeamType(string teamName, TeamTypeMapping globalConfigurationTeamTypeMapping)
        {
            if (globalConfigurationTeamTypeMapping.Sale.Contains(teamName)) return TeamType.Sale;
            if (globalConfigurationTeamTypeMapping.Affiliate.Contains(teamName)) return TeamType.Affiliate;
            if (globalConfigurationTeamTypeMapping.Seeding.Contains(teamName)) return TeamType.Seeding;
            if (globalConfigurationTeamTypeMapping.Content.Contains(teamName)) return TeamType.Content;
            if (globalConfigurationTeamTypeMapping.Tiktok.Contains(teamName)) return TeamType.Tiktok;

            return TeamType.Unknown;
        }

        private async Task<List<(OrganizationUnit, List<IdentityUser>, List<IdentityUser>)>> GetDailyEvaluationTeams()
        {
            var results = new List<(OrganizationUnit, List<IdentityUser>, List<IdentityUser>)>();
            var teams = await _organizationUnitsRepository.GetListAsync();
            var leaderRole = await _identityRoleManager.FindByNameAsync(RoleConsts.Leader);
            var managerRole = await _identityRoleManager.FindByNameAsync(RoleConsts.Manager);
            foreach (var team in teams)
            {
                var teamUsers = await _userManager.GetUsersInOrganizationUnitAsync(team);
                var leaders = teamUsers.Where(x => x.IsInRole(leaderRole.Id) || x.IsInRole(managerRole.Id)).ToList();
                if (leaders.Any())
                {
                    results.Add(new(team, leaders, teamUsers.Except(leaders).ToList()));
                }
            }

            return results;
        }
    }

    public class UnqualifiedDailyKPIResult
    {
        public List<IdentityUser> Leaders { get; set; }
        public List<DailyKPIResult> UnqualifiedResults { get; set; }
    }

    public class DailyKPIResult
    {
        public IdentityUser User { get; set; }
        public int RequiredQuantity { get; set; }
        public int ActualQuantity { get; set; }
    }
}