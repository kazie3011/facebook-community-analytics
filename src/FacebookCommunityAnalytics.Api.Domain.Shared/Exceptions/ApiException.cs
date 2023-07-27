using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Exceptions
{
    public class ApiException: UserFriendlyException
    {
        public ApiException(string message, string code = null, string details = null, Exception innerException = null, LogLevel logLevel = LogLevel.Warning) : base(message, code, details, innerException, logLevel)
        {
        }

        public ApiException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }
    }
}
