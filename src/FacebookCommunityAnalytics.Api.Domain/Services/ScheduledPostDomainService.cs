using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IScheduledPostDomainService : IDomainService
    {
        Task<List<SchedulePostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(GetScheduledPostInput input);
        Task<List<ScheduledPost>> GetListScheduledPostHadPosted();
    }
    public class ScheduledPostDomainService : BaseDomainService, IScheduledPostDomainService
    {
        private readonly IScheduledPostRepository _scheduledPostRepository;

        public ScheduledPostDomainService(IScheduledPostRepository scheduledPostRepository)
        {
            _scheduledPostRepository = scheduledPostRepository;
        }

        public async Task<List<SchedulePostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(GetScheduledPostInput input)
        {
            //Todoo: ScheduledPost Update Filter
            return await _scheduledPostRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Content,
                input.IsAutoPost, input.ScheduledPostDateTimeMin,
                input.ScheduledPostDateTimeMax, input.PostedAtMin, input.PostedAtMax, input.GroupId);
        }

        public async Task<List<ScheduledPost>> GetListScheduledPostHadPosted()
        {
            var schedulePosts = await _scheduledPostRepository.GetListAsync(isPosted: false);
            return schedulePosts.Where(_ => _.PostedAt != null).ToList();
        }
    }
}
