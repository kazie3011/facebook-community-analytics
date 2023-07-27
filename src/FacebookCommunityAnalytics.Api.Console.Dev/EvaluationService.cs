using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class EvaluationService : ITransientDependency
    {
        private readonly IOrganizationDomainService _orgDomain;
        private readonly IUserDomainService _userDomain;
        private readonly IStaffEvaluationRepository _evalRepo;
        

        public EvaluationService(IOrganizationDomainService orgDomain, IStaffEvaluationRepository evalRepo, IUserDomainService userDomain)
        {
            _orgDomain = orgDomain;
            _evalRepo = evalRepo;
            _userDomain = userDomain;
        }

        public async Task Exec()
        {
            await MoveEvaluation();
        }

        private async Task MoveEvaluation()
        {
            var username = "nhivu";
            var month = 1;
            var year = 2022;
            var newTeamName = TeamMemberConsts.Operation;
            
            var team = await _orgDomain.GetTeam(newTeamName);
            if (team is null) return;
            var newTeamId = team.Id;

            var user = await _userDomain.GetByUsername(username);
            if(user is null) return;

            var evaluation = await _evalRepo.FindAsync(x => x.Month == month && x.Year == year && x.AppUserId == user.Id);
            evaluation.TeamId = newTeamId;
            await _evalRepo.UpdateAsync(evaluation);
        }
    }
}