using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    public interface IAutoPostFacebookDomainService : IDomainService
    {
        public Task<IList<AutoPostFacebookNotDone>> GetUnAutoPostFacebooksAsync();
        public Task UpdateLikeCommentAsync(Guid autoPostFacebookId, int numberLike, int numberComment);
    }

    public class AutoPostFacebookDomainService : BaseDomainService, IAutoPostFacebookDomainService
    {
        private readonly IRepository<AutoPostFacebook> _autoPostFacebookRepository;

        public AutoPostFacebookDomainService(IRepository<AutoPostFacebook> autoPostFacebookRepository)
        {
            _autoPostFacebookRepository = autoPostFacebookRepository;
        }

        public async Task<IList<AutoPostFacebookNotDone>> GetUnAutoPostFacebooksAsync()
        {
            var postFacebooks = await _autoPostFacebookRepository.Where(x => !x.IsDone).ToDynamicListAsync<AutoPostFacebook>();
            return postFacebooks.Select
                (
                    x => new AutoPostFacebookNotDone
                    {
                        Id = x.Id,
                        Url = x.Url,
                        Comments = x.Comments,
                        CurrentComment = x.CurrentComment,
                        CurrentLike = x.CurrentLike,
                        TotalComment = x.TotalComment,
                        TotalLike = x.TotalLike,
                        IsDone = x.IsDone,
                        NeedLike = x.TotalLike - x.CurrentLike,
                        NeedComment = x.TotalComment - x.CurrentComment
                    }
                )
                .ToList();
        }

        public async Task UpdateLikeCommentAsync(Guid autoPostFacebookId, int numberLike, int numberComment)
        {
            var autoPostFacebook = await _autoPostFacebookRepository.GetAsync(x => x.Id == autoPostFacebookId);
            if (autoPostFacebook != null)
            {
                autoPostFacebook.CurrentLike += numberLike;
                autoPostFacebook.CurrentComment += numberComment;

                if (autoPostFacebook.CurrentLike >= autoPostFacebook.TotalLike && autoPostFacebook.CurrentComment >= autoPostFacebook.TotalComment)
                {
                    autoPostFacebook.IsDone = true;
                }

                await _autoPostFacebookRepository.UpdateAsync(autoPostFacebook);
            }
        }
    }
}