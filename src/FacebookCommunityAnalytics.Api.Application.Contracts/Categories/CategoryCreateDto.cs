using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; }
    }
}