using Volo.Abp.Settings;

namespace FacebookCommunityAnalytics.Api.Settings
{
    public class ApiSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(ApiSettings.MySetting1));

            context.Add(
            );
        }
    }
}
