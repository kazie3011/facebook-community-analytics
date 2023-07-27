using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class GetProxiesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Ip { get; set; }
        public int? PortMin { get; set; }
        public int? PortMax { get; set; }
        public string Protocol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LastPingDateTimeMin { get; set; }
        public DateTime? LastPingDateTimeMax { get; set; }
        public bool? IsActive { get; set; }

        public GetProxiesInput()
        {

        }
    }
}