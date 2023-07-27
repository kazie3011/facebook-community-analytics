using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }

        public GetCategoriesInput()
        {

        }
    }
}