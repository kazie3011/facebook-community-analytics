using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class PayrollDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public List<TeamBonuses> TeamBonuses { get; set; }
        public bool IsCompensation { get; set; }
        public PayrollDto ()
        {
            TeamBonuses = new List<TeamBonuses>();
        }
    }
}