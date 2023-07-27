using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class BaseScheduledPostValidator<T> : BaseAbstractValidator<T> where T : class
    {
        private readonly IScheduledPostRepository _scheduledPostRepository;
        public BaseScheduledPostValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv, IScheduledPostRepository scheduledPostRepository) : base(l, lv)
        {
            _scheduledPostRepository = scheduledPostRepository;
        }

        protected bool IsValidContent(string content)
        {
            return content.IsNotNullOrWhiteSpace();
        }

        protected bool IsValidGroup(string group)
        {
            return group.IsNotNullOrWhiteSpace();
        }

        //protected bool IsValidScheduledDateTime(DateTime? dateTime)
        //{
        //    return dateTime == null || dateTime.Value.Date >= DateTime.UTCNow.Date;
        //}
    }
    public class ScheduledPostCreateDtoValidator : BaseScheduledPostValidator<ScheduledPostCreateDto>
    {
        public ScheduledPostCreateDtoValidator(IScheduledPostRepository scheduledPostRepository, IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv) : base(l, lv, scheduledPostRepository)
        {
            RuleFor(x => x.Content).Must(IsValidContent)
                .WithMessage(LV[ApiDomainErrorCodes.ScheduledPosts.InValidContent]);
            RuleFor(x => x.GroupIds).Must(IsValidGroup)
                .WithMessage(LV[ApiDomainErrorCodes.ScheduledPosts.InValidGroup]);
            //RuleFor(x => x.ScheduledPostDateTime).Must(IsValidScheduledDateTime)
            //    .WithMessage(LV[ApiDomainErrorCodes.ScheduledPost.InValidDateTime]);
        }

    }

    public class ScheduledPostUpdateDtoValidator : BaseScheduledPostValidator<ScheduledPostUpdateDto>
    {
        public ScheduledPostUpdateDtoValidator(IScheduledPostRepository scheduledPostRepository, IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv) : base(l, lv, scheduledPostRepository)
        {
            RuleFor(x => x.Content).Must(IsValidContent)
                .WithMessage(LV[ApiDomainErrorCodes.ScheduledPosts.InValidContent]);
            RuleFor(x => x.GroupIds).Must(IsValidGroup)
                .WithMessage(LV[ApiDomainErrorCodes.ScheduledPosts.InValidGroup]);

        }
    }
}
