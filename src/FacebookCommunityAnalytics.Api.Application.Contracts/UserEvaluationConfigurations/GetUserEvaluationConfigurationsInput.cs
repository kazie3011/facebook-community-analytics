using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserEvaluationConfigurations
{
    public class GetUserEvaluationConfigurationsInput
    {
        public Guid? TeamId { get; set; }
        public List<Guid> TeamIds { get; set; }
    }
}