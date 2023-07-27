using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class Partner : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        [CanBeNull]
        public virtual string Url { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        public virtual PartnerType PartnerType { get; set; }
        public virtual bool IsActive { get; set; }

        public CompanyInfo CompanyInfo { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public List<Comment> Comments { get; set; }
        
        public List<Guid> PartnerUserIds { get; set; }
        public List<Guid> PartnerIds { get; set; }
        
        public Partner()
        {
            CompanyInfo = new CompanyInfo();
            PersonalInfo = new PersonalInfo();
            PaymentInfo = new PaymentInfo();
            Comments = new List<Comment>();
            PartnerUserIds = new List<Guid>();
            PartnerIds = new List<Guid>();
        }

        public Partner(Guid id, string name, string description, string url, string code, bool isActive)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Description = description;
            Url = url;
            Code = code;
            IsActive = isActive;
        }
    }
}