using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.ApiNotifications
{
    public class SendMessageInput
    {
        public SendMessageInput()
        {
            SenderId = Guid.Empty;
        }

        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }

        public bool IsLocalized { get; set; }

        public IDictionary<string, string> Extends { get; set; }
    }
}
