using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.TeamMembers
{
    public class TeamMemberDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserPosition Position { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsGDLStaff { get; set; }
        public bool IsCalculatePayrollUser { get; set; }
        public List<OrganizationUnitDto> Teams { get; set; }

        public TeamMemberDto()
        {
            Teams = new List<OrganizationUnitDto>();
        }
    }
}