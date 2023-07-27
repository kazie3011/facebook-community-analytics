using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    public class GetMembersApiRequest
    {
        public string FilterText { get; set; }
        public Guid? TeamId { get; set; }
        [CanBeNull] public List<Guid> TeamIds { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool IsActiveUser { get; set; } = true;
        public string TeamName { get; set; }
        public UserPosition? UserPosition { get; set; }
    }

    public class AssignTeamApiRequest
    {
        public List<Guid> UserIds { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public bool IsTeamAssigned { get; set; }
    }

    public class UpdateMemberConfigApiRequest
    {
        public List<Guid> UserIds { get; set; }
        public bool? IsSystemUsers { get; set; }
        public bool? IsGDLUser { get; set; }
        public bool? IsCalculatePayrollUsers { get; set; }
        public bool? IsActiveUsers { get; set; }
    }
    
}