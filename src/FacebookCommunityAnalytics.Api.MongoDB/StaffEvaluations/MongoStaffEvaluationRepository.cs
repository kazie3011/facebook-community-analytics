using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class MongoStaffEvaluationRepository : MongoDbRepositoryBase<ApiMongoDbContext, StaffEvaluation, Guid>, IStaffEvaluationRepository
    {
        public MongoStaffEvaluationRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<StaffEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filter = null,
            Guid? teamId = null,
            List<Guid> teamIds = null,
            Guid? appUserId = null,
            int? month = null,
            int? year = null,
            StaffEvaluationStatus? staffEvaluationStatus = null,
            bool? IsTikTokEvaluation = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                teamId,
                teamIds,
                appUserId,
                month: month,
                year: year,
                staffEvaluationStatus: staffEvaluationStatus,
                IsTikTokEvaluation: IsTikTokEvaluation,
                cancellationToken: cancellationToken
            );

            // query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? StaffEvaluationConsts.GetDefaultSorting(true) : sorting);
                // .As<IMongoQueryable<StaffEvaluationWithNavigationProperties>>()
                // .PageBy<StaffEvaluationWithNavigationProperties, IMongoQueryable<StaffEvaluationWithNavigationProperties>>(skipCount, maxResultCount);
            
            var staffEvaluations = query.Skip(skipCount).Take(maxResultCount).ToList();
            return staffEvaluations;
        }

        public async Task<long> GetCountAsync(
            string filter = null,
            Guid? teamId = null,
            List<Guid> teamIds = null,
            Guid? appUserId = null,
            int? month = null,
            int? year = null,
            StaffEvaluationStatus? staffEvaluationStatus = null,
            bool? IsTikTokEvaluation = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyFilter
            (
                filter,
                teamId,
                teamIds,
                appUserId,
                month: month,
                year: year,
                staffEvaluationStatus: staffEvaluationStatus,
                IsTikTokEvaluation: IsTikTokEvaluation,
                cancellationToken: cancellationToken
            );
            return query.LongCount();
        }


        private async Task<IQueryable<StaffEvaluationWithNavigationProperties>> ApplyFilter(
            string filter = null,
            Guid? teamId = null,
            List<Guid> teamIds = null,
            Guid? appUserId = null,
            int? month = null,
            int? year = null,
            StaffEvaluationStatus? staffEvaluationStatus = null,
            bool? IsTikTokEvaluation = null,
            CancellationToken cancellationToken = default)
        {
            var staffEvaluations = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
            var dbContext = await GetDbContextAsync(cancellationToken);

            var query = from ct in staffEvaluations
                join e in Queryable.Where(dbContext.Users.AsQueryable(), _ => !_.IsDeleted) on ct.AppUserId equals e.Id into t2
                from pn in t2.DefaultIfEmpty()
                join og in Queryable.Where(dbContext.OrganizationUnits.AsQueryable(), _=>!_.IsDeleted) on ct.TeamId equals og.Id into t3
                from ogt in t3.DefaultIfEmpty()
                join info in Queryable.Where(dbContext.UserInfos.AsQueryable(), _=>!_.IsDeleted) on ct.AppUserId equals info.AppUserId into t4
                from infos in t4.DefaultIfEmpty()
                join g in Queryable.Where(dbContext.Groups.AsQueryable(), _=>!_.IsDeleted) on ct.CommunityId equals g.Id into t5
                from gs in t5.DefaultIfEmpty()
                
                select new StaffEvaluationWithNavigationProperties()
                {
                    StaffEvaluation = ct,
                    AppUser = pn,
                    OrganizationUnit = ogt,
                    Info = infos,
                    Group = gs,
                };

            if (filter.IsNotNullOrWhiteSpace())
            {
                filter = filter.Trim().ToLower();
            }

            if (IsTikTokEvaluation.HasValue && IsTikTokEvaluation.Value)
            {
                query = query.AsQueryable().Where( e => e.Group != null && e.StaffEvaluation.CommunityId.IsNotNullOrEmpty());
            }
            else
            {
                query = query.AsQueryable().Where( e => e.StaffEvaluation.CommunityId.IsNullOrEmpty());
            }
            
            return query.AsQueryable()
                .WhereIf(filter.IsNotNullOrWhiteSpace(), e => e.AppUser != null && (e.AppUser.UserName.ToLower().Contains(filter)))
                .WhereIf(teamId.IsNotNullOrEmpty(), e => e.StaffEvaluation.TeamId.IsNotNullOrEmpty() && e.StaffEvaluation.TeamId == teamId)
                .WhereIf(appUserId != null, e => e.StaffEvaluation.AppUserId.IsNotNullOrEmpty() && e.StaffEvaluation.AppUserId == appUserId)
                .WhereIf(month.HasValue, e => e.StaffEvaluation != null && e.StaffEvaluation.Month == month)
                .WhereIf(year.HasValue, e =>e.StaffEvaluation != null && e.StaffEvaluation.Year == year)
                .WhereIf(staffEvaluationStatus.HasValue, e => e.StaffEvaluation != null && e.StaffEvaluation.StaffEvaluationStatus == staffEvaluationStatus)
                .WhereIf(teamIds.IsNotNullOrEmpty(), e => e.StaffEvaluation.TeamId != null && teamIds.Contains(e.StaffEvaluation.TeamId.Value));
        }
        
        public async Task<StaffEvaluationWithNavigationProperties> GetWithNavigationPropertiesByUserAsync(Guid userId, int year, int month, CancellationToken cancellationToken = default)
        {
            var query = (await GetMongoQueryableAsync(cancellationToken));
           var  staffEvaluation = await query.AsQueryable().WhereIf(userId != Guid.Empty, x => x.AppUserId == userId)
                .WhereIf(month > 0, x=>x.Month == month)
                .WhereIf(year > 0, x=>x.Year == year).As<IMongoQueryable<StaffEvaluation>>().FirstOrDefaultAsync(cancellationToken);
                
           if (staffEvaluation != null)
           {
               var dbContext = await GetDbContextAsync(cancellationToken);
               var appUser = await dbContext.Users.AsQueryable().FirstOrDefaultAsync(x => x.Id == staffEvaluation.AppUserId, cancellationToken: cancellationToken);
               var organizationUnit = await dbContext.OrganizationUnits.AsQueryable().FirstOrDefaultAsync(x => x.Id == staffEvaluation.TeamId, cancellationToken: cancellationToken);
               return new StaffEvaluationWithNavigationProperties()
               {
                   StaffEvaluation = staffEvaluation,
                   AppUser = appUser,
                   OrganizationUnit = organizationUnit
               };
           }

           return null;
        }
        
    }
}