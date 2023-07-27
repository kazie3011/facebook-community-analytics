using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public class CategoryDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
    }
}