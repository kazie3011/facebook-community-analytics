using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Anonymous
{
    public interface IAnonymousAppService : IApplicationService
    {
        void ProcessEmail(IFormCollection formCollection);
        string GetSecurityCode();
    }
}