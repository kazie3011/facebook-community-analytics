using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class ProxyCreateDto
    {
        [Required]
        public string Ip { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string Protocol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastPingDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}