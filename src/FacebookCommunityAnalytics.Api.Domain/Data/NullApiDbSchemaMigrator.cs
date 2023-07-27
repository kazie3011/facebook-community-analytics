using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Data
{
    /* This is used if database provider does't define
     * IApiDbSchemaMigrator implementation.
     */
    public class NullApiDbSchemaMigrator : IApiDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}