using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookCommunityAnalytics.Api.Notifications.Emails
{
    public class EmailModelBase
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsLocalized { get; set; }

        public IDictionary<string, string> Extends { get; set; }

        public EmailModelBase()
        {
            SenderId = Guid.Empty;
            CreatedAt = DateTime.UtcNow;
            Extends = new Dictionary<string, string>();
        }
    }

    public class EmailTemplateConsts
    {
        public const string Sample = "Sample";
    }
}
