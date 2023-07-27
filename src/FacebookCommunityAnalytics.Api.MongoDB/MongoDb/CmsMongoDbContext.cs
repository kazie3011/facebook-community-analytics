using FacebookCommunityAnalytics.Api.Cms.Pages;
using FacebookCommunityAnalytics.Api.CmsSites;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.CmsKit.MongoDB;

namespace FacebookCommunityAnalytics.Api.MongoDB
{
    [ConnectionStringName("CmsKit")]
    public class CmsMongoDbContext : AbpMongoDbContext
    {
        public IMongoCollection<CmsSite> CmsSites => Collection<CmsSite>();
        public IMongoCollection<CmsPage> CmsPages => Collection<CmsPage>();
        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.Entity<CmsSite>(b => { b.CollectionName = ApiConsts.DbCmsTablePrefix + "CmsSites"; });
            modelBuilder.Entity<CmsPage>(b => { b.CollectionName = ApiConsts.DbCmsTablePrefix + "CmsPages"; });
        } 
    }
}