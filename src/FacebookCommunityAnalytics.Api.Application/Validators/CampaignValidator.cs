using System;
using System.Net.Mail;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class CampaignValidator <T> : BaseAbstractValidator<T> where T : class
    {
        private readonly ICampaignRepository _campaignRepository;
        public CampaignValidator(
            IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv,
            ICampaignRepository campaignRepository) : base(l, lv)
        {
            _campaignRepository = campaignRepository;
        }

        protected bool IsValidEmails(string emails)
        {
            // Note: temporary remove this validation since we need to validate multiple emails in one string value
            // if (emails.IsNullOrEmpty()) return true;
            //
            // foreach (var item in emails.Split(','))
            // {
            //     if (!StringHelper.IsValidEmail(item.Trim()))
            //     {
            //         return false;
            //     }
            // }
            return true;
        }
    }

    public class CampaignCreateDtoValidator : CampaignValidator<CampaignCreateDto>
    {
        public CampaignCreateDtoValidator(
            ICampaignRepository campaignRepository,
            IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv) : base(l, lv, campaignRepository)
        {
            RuleFor(x => x.Emails).Must(IsValidEmails)
                .WithMessage(LV[ApiDomainErrorCodes.Campaign.InvalidEmails]);
        }
    }
    
    public class CampaignUpdateDtoValidator : CampaignValidator<CampaignUpdateDto>
    {
        public CampaignUpdateDtoValidator(
            ICampaignRepository campaignRepository,
            IStringLocalizer<ApiResource> l,
            IStringLocalizer<ApiDomainResource> lv) : base(l, lv, campaignRepository)
        {
            RuleFor(x => x.Emails).Must(IsValidEmails)
                .WithMessage(LV[ApiDomainErrorCodes.Campaign.InvalidEmails]);
        }
    }
}