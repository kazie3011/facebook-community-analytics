using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class Proxy : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Ip { get; set; }

        public virtual int Port { get; set; }

        [NotNull]
        public virtual string Protocol { get; set; }

        [CanBeNull]
        public virtual string Username { get; set; }

        [CanBeNull]
        public virtual string Password { get; set; }

        public virtual DateTime LastPingDateTime { get; set; }

        public virtual bool IsActive { get; set; }

        public Proxy()
        {

        }

        public Proxy(Guid id, string ip, int port, string protocol, string username, string password, DateTime lastPingDateTime, bool isActive)
        {
            Id = id;
            Check.NotNull(ip, nameof(ip));
            Check.NotNull(protocol, nameof(protocol));
            Ip = ip;
            Port = port;
            Protocol = protocol;
            Username = username;
            Password = password;
            LastPingDateTime = lastPingDateTime;
            IsActive = isActive;
        }
    }
}