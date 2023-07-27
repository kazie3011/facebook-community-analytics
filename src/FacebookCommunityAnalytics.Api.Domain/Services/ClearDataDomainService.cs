using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserInfos;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IClearDataDomainService : IDomainService
    {
        Task CleanUpData(int? days = null);
        Task CleanUserInfos();
    }
    
    public class ClearDataDomainService : DomainService, IClearDataDomainService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IRepository<Tiktok, Guid> _tiktokRepository;


        public ClearDataDomainService(IUserInfoRepository userInfoRepository,
            IAuditLogRepository auditLogRepository,
            IPostRepository postRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IRepository<Tiktok, Guid> tiktokRepository)
        {
            _userInfoRepository = userInfoRepository;
            _auditLogRepository = auditLogRepository;
            _postRepository = postRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _tiktokRepository = tiktokRepository;
        }

        public async Task CleanUserInfos()
        {
            var batchSize = 100;

            var userInfos = await _userInfoRepository.GetListWithNavigationPropertiesExtendAsync();
            var userInfoGroups = userInfos.GroupBy(_ => _.AppUser.UserName).ToList();
            var deleteUserInfos = new List<UserInfo>();
            foreach (var item in userInfoGroups)
            {
                if (item.Count() > 1)
                {
                    var temp = item.OrderByDescending(_ => _.UserInfo.LastModificationTime).ToList();

                    temp.Remove(temp.First());
                    deleteUserInfos.AddRange(temp.Select(_ => _.UserInfo));
                }
            }
            foreach (var batch in deleteUserInfos.Partition(batchSize)) { await _userInfoRepository.DeleteManyAsync(batch); }

            var userInfo_CodeGroups = userInfos.GroupBy(x => x.UserInfo.Code).ToList();
            foreach (var item in userInfo_CodeGroups)
            {
                if (item.Count() > 1)
                {
                    var theRest = item.Skip(1).ToList();
                    foreach (var i in theRest)
                    {
                        var currentCodeCount = await _userInfoRepository.GetCurrentUserCode();
                        i.UserInfo.Code = (currentCodeCount + 1).ToString();
                        await _userInfoRepository.UpdateAsync(i.UserInfo);
                    }
                }
            }
        }
        

        public async Task CleanUpData(int? days = null)
        {
            var toDateTime = DateTime.UtcNow;
            var fromDateTime = new DateTime();
            if (days.HasValue) fromDateTime = toDateTime.AddDays(-days.Value).Date;

            var auditLogsKeepDays = 1;
            var batchSize = 100;

            var oldAuditLogs = (await _auditLogRepository.GetListAsync(x => x.ExecutionTime < toDateTime.AddDays(-auditLogsKeepDays))).ToList();
            foreach (var batch in oldAuditLogs.Partition(1000)) { await _auditLogRepository.DeleteManyAsync(batch); }

            var posts = await _postRepository.GetListAsync(submissionDateTimeMin: fromDateTime, submissionDateTimeMax: toDateTime);
            var fbPosts = posts.Where(_ => _.Fid.IsNotNullOrEmpty()).ToList();
            var groupByFbPosts = fbPosts.GroupBy(_ => _.Fid.Trim()).ToList();
            var deleteFbPosts = new List<Post>();
            foreach (var item in groupByFbPosts)
            {
                if (item.Count() > 1)
                {
                    var temp = item.OrderByDescending(_ => _.TotalCount).ToList();

                    temp.Remove(temp.First());
                    deleteFbPosts.AddRange(temp);
                }
            }

            foreach (var batch in deleteFbPosts.Partition(batchSize)) { await _postRepository.DeleteManyAsync(batch); }

            await CleanUserInfos();

            var userAffiliates = await _userAffiliateRepository.GetListAsync(createdAtMin: fromDateTime, createdAtMax: toDateTime);
            var groupByUserAffs = userAffiliates.GroupBy(_ => _.AffiliateUrl.ToLower().Trim()).ToList();
            var deleteUserAffs = new List<UserAffiliate>();
            foreach (var item in groupByUserAffs)
            {
                if (item.Count() > 1)
                {
                    var temp = item.OrderByDescending(_ => _.AffConversionModel.ClickCount).ToList();

                    temp.Remove(temp.First());
                    deleteUserAffs.AddRange(temp);
                }
            }

            foreach (var batch in deleteUserAffs.Partition(batchSize)) { await _userAffiliateRepository.DeleteManyAsync(batch);}
            
            var tiktokList = await _tiktokRepository.ToListAsync();
            var tiktokGroup = tiktokList.GroupBy(_ => _.VideoId);
            var deleteTiktokVideo = new List<Tiktok>();
            foreach (var items in tiktokGroup)
            {
                if (items.Count() > 1)
                {
                    var temp = items.OrderByDescending(_ => _.ViewCount).ToList();

                    temp.Remove(temp.First());
                    deleteTiktokVideo.AddRange(temp);
                }
            }
            foreach (var batch in deleteTiktokVideo.Partition(batchSize)) { await _tiktokRepository.DeleteManyAsync(batch);}
            
        }
    }
}