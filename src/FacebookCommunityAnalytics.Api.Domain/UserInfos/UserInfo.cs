using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfo : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        [CanBeNull]
        public virtual string IdentityNumber { get; set; }

        [CanBeNull]
        public virtual string Facebook { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual DateTime? JoinedDateTime { get; set; }

        public virtual DateTime? PromotedDateTime { get; set; }
        public virtual DateTime? LeaderPromotedDateTime { get; set; }

        public virtual double AffiliateMultiplier { get; set; }
        public virtual double SeedingMultiplier { get; set; }

        public virtual ContentRoleType ContentRoleType { get; set; }

        public virtual bool IsGDLStaff { get; set; }
        public virtual bool IsSystemUser { get; set; }
        public bool EnablePayrollCalculation { get; set; } = true;
        public double AverageReactionTop100Post { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? AppUserId { get; set; }
        public List<UserInfoAccount> Accounts { get; set; }
        public UserPosition UserPosition { get; set; }
        public Guid? AvatarMediaId { get; set; }
        public Guid? MainTeamId { get; set; }
        public UserInfo()
        {
            Accounts = new List<UserInfoAccount>();
            IsActive = true;
        }

        public UserInfo(Guid id, string code, string identityNumber, string facebook, double affiliateMultiplier, double seedingMultiplier, ContentRoleType contentRoleType, bool isGDLStaff, DateTime? dateOfBirth = null, DateTime? joinedDateTime = null, DateTime? promotedDateTime = null, bool isActive = true, bool isSystemUser = false, bool enablePayrollCalculation = true)
        {
            Id = id;
            Code = code;
            IdentityNumber = identityNumber;
            Facebook = facebook;
            AffiliateMultiplier = affiliateMultiplier;
            SeedingMultiplier = seedingMultiplier;
            ContentRoleType = contentRoleType;
            IsGDLStaff = isGDLStaff;
            DateOfBirth = dateOfBirth;
            JoinedDateTime = joinedDateTime;
            PromotedDateTime = promotedDateTime;
            IsActive = isActive;
            IsSystemUser = isSystemUser;
            EnablePayrollCalculation = enablePayrollCalculation;
            Accounts = new List<UserInfoAccount>();
        }
    }
}