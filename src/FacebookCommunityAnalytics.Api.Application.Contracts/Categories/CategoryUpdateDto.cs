using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.Categories
{
    public class CategoryUpdateDto
    {
        [Required]
        public string Name { get; set; }
    }
}