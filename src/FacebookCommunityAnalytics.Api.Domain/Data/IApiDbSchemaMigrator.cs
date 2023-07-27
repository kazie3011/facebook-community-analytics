using System.Threading.Tasks;

namespace FacebookCommunityAnalytics.Api.Data
{
    public interface IApiDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}