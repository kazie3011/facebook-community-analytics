using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class Payroll : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        [CanBeNull]
        public virtual string Title { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        public virtual DateTime? FromDateTime { get; set; }

        public virtual DateTime? ToDateTime { get; set; }
        public virtual List<TeamBonuses> TeamBonuses { get; set; }

        public bool IsCompensation { get; set; }
        public Payroll()
        {
            TeamBonuses = new List<TeamBonuses>();
        }

        public Payroll(Guid id, string code, string title, string description, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            Id = id;
            Code = code;
            Title = title;
            Description = description;
            FromDateTime = fromDateTime;
            ToDateTime = toDateTime;
            TeamBonuses = new List<TeamBonuses>();
        }
    }

}