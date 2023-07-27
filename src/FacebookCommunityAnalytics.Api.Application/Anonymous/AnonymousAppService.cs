using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Anonymous
{
    [RemoteService(IsEnabled = false)]
    [AllowAnonymous]
    public class AnonymousAppService : ApiAppService, IAnonymousAppService
    {
        public static string SecurityCode;
        
        public void ProcessEmail(IFormCollection formCollection)
        {
            if (formCollection.ContainsKey("text"))
            {
                StringValues textData;
                var isValid = formCollection.TryGetValue("text", out textData);
                if (isValid && textData.Any())
                {
                    var textBody = textData.First();
                    if (textBody.Contains("Please use the following security code for the Microsoft"))
                    {
                        var regex = new Regex("([0-9])\\w+");
                        var match = regex.Match(textBody);

                        var securityCode = match.Success ? match.Groups[0].Value : string.Empty;

                        SecurityCode = securityCode;
                    }
                }
            }
        }

        public string GetSecurityCode()
        {
            var securityCode = SecurityCode;
            SecurityCode = string.Empty;
            return securityCode;
        }
    }
}