using FacebookCommunityAnalytics.Api.Core.Helpers;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class ProxyImportInput
    {
        public ProxyImportInput()
        {
            Items = new List<ProxyImportDto>();
        }
        public List<ProxyImportDto> Items { get; set; }
    }

    public class ProxyImportDto
    {
        [Column(1)]
        public string Protocol { get; set; }

        [Column(2)]
        public string Ip { get; set; }

        [Column(3)]
        public int Port { get; set; }

        [Column(4)]
        public string Username { get; set; }

        [Column(5)]
        public string Password { get; set; }
    }
}
