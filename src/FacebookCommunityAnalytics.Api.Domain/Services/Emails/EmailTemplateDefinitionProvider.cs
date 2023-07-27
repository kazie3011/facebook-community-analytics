using FacebookCommunityAnalytics.Api.Notifications.Emails;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.Services.Emails
{
    public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(
                new TemplateDefinition(EmailTemplateConsts.Sample)
                    .WithVirtualFilePath(
                        "/Emails/Sample.tpl",
                        isInlineLocalized: true
                    )
            );
        }
    }
}
