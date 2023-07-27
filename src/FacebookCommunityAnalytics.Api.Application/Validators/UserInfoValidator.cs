using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.UserInfos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class UserInfoValidator<T> : BaseAbstractValidator<T> where T : class
    {
        protected readonly IUserInfoRepository UserInfoRepository;

        public UserInfoValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv, IUserInfoRepository userInfoRepository) : base(l, lv)
        {
            UserInfoRepository = userInfoRepository;
        }
        
    }

    public class UserInfoUpdateDtoValidator : UserInfoValidator<UserInfoUpdateDto>
    {
        public UserInfoUpdateDtoValidator(IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv, IUserInfoRepository userInfoRepository) : base(l, lv, userInfoRepository)
        {
            
        }
    }
}
