using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class BaseUserAfffiliateValidator<T> : BaseAbstractValidator<T> where T : class
    {
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly LazadaConfiguration _lazadaConfiguration;


        public BaseUserAfffiliateValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv,
            IUserAffiliateRepository userAffiliateRepository,
           GlobalConfiguration globalConfiguration) : base(l, lv)
        {
            _userAffiliateRepository = userAffiliateRepository;
            _lazadaConfiguration = globalConfiguration.LazadaConfiguration;
        }

        protected bool IsValidLink(string link)
        {
            return link.IsNotNullOrWhiteSpace();
        }
        
        protected bool IsValidShortlink(string shortLink)
        {
            return shortLink.IsNotNullOrWhiteSpace();
        }

        protected bool BeValidLazadaUrl(string link)
        {
            return link.StartsWith($"{_lazadaConfiguration.BaseLink}?url={_lazadaConfiguration.BaseProductLink}");
        }
    }

    public class UserAffiliateCreateDtoValidator : BaseUserAfffiliateValidator<UserAffiliateCreateDto>
    {
        public UserAffiliateCreateDtoValidator(IUserAffiliateRepository userAffiliateRepository,
            IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv,
            GlobalConfiguration globalConfiguration
            ) : base(l, lv,
            userAffiliateRepository, globalConfiguration)
        {
            RuleFor(x => x.AffiliateUrl).Must(IsValidShortlink)
                .WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidShortlink]);

            // RuleFor(x => x.Url).Must(BeValidLazadaUrl).WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidLazadaUrl])
            //     .When(x => x.MarketplaceType == MarketplaceType.Lazada);
            
            RuleFor(x => x.Url).Must(IsValidLink)
                .WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidShortlink]);
        }
    }

    public class UserAffiliateUpdateDtoValidator : BaseUserAfffiliateValidator<UserAffiliateUpdateDto>
    {
        public UserAffiliateUpdateDtoValidator(IUserAffiliateRepository userAffiliateRepository,
            IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv,
            GlobalConfiguration globalConfiguration) : base(l, lv, userAffiliateRepository, globalConfiguration)
        {
            RuleFor(x => x.AffiliateUrl).Must(IsValidShortlink)
                .WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidShortlink]);

            // RuleFor(x => x.Url).Must(BeValidLazadaUrl).WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidLazadaUrl])
            //     .When(x => x.MarketplaceType == MarketplaceType.Lazada);
            
            RuleFor(x => x.Url).Must(IsValidLink)
                .WithMessage(LV[ApiDomainErrorCodes.UserAffiliates.InvalidUrl]);
        }
    }
}
