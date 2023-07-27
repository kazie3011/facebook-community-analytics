using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.ApiNotifications
{
    public class ReceivedMessageEto
    {
        public string ReceivedText { get; set; }

        public Guid TargetUserId { get; set; }

        public string SenderUserName { get; set; }

        public NotifyType NotifyType { get; set; }

        public ReceivedMessageEto(Guid targetUserId, string senderUserName, string receivedText, NotifyType notifyType = Core.Enums.NotifyType.Info)
        {
            ReceivedText = receivedText;
            TargetUserId = targetUserId;
            SenderUserName = senderUserName;
            NotifyType = notifyType;
        }
    }
}