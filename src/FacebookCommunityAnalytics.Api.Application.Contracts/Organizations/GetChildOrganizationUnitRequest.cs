using System;

namespace FacebookCommunityAnalytics.Api.Organizations
{
    public class GetChildOrganizationUnitRequest
    {
        public Guid? UserId { get; set; }
        public bool? IsGDLNode { get; set; }
    }
}