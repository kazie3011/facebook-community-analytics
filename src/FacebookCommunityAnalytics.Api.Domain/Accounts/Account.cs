using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Accounts
{
    public class Account : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Username { get; set; }

        [NotNull]
        public virtual string Password { get; set; }

        [CanBeNull]
        public virtual string TwoFactorCode { get; set; }

        public virtual AccountType AccountType { get; set; }

        public virtual AccountStatus AccountStatus { get; set; }
        public virtual AccountCountry AccountCountry { get; set; }

        public virtual bool IsActive { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }

        public Account()
        {

        }

        public Account(Guid id, string username, string password, string twoFactorCode, AccountType accountType, AccountStatus accountStatus, bool isActive, string description, string email, string emailPassword)
        {
            Id = id;
            Check.NotNull(username, nameof(username));
            Check.NotNull(password, nameof(password));
            Username = username;
            Password = password;
            TwoFactorCode = twoFactorCode;
            AccountType = accountType;
            AccountStatus = accountStatus;
            IsActive = isActive;
            Description = description;
            Email = email;
            EmailPassword = emailPassword;
        }
    }
}