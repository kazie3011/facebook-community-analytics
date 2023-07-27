using FacebookCommunityAnalytics.Api.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class BaseAbstractValidator<T> : AbstractValidator<T>
    {
        protected readonly IStringLocalizer<ApiResource> L;
        protected readonly IStringLocalizer<ApiDomainResource> LV;
        public BaseAbstractValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv)
        {
            L = l;
            LV = lv;
        }
    }
}
