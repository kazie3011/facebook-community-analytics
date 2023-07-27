using FacebookCommunityAnalytics.Api.MongoDB;
using Xunit;

namespace FacebookCommunityAnalytics.Api
{
    [CollectionDefinition(ApiTestConsts.CollectionDefinitionName)]
    public class ApiApplicationCollection : ApiMongoDbCollectionFixtureBase
    {

    }
}
