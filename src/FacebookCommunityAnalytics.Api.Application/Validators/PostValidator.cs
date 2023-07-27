using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Posts;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class BasePostValidator<T> : BaseAbstractValidator<T> where T : class
    {
        protected readonly IPostRepository PostRepository;

        protected List<string> InvalidUrls;
        protected List<string> ExistingUrls;
        protected List<string> IsNotSameTypeUrl;
        protected bool ismaxUrlReached;
        protected int MaxUrlReached = 1000;

        public BasePostValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv, IPostRepository postRepository) : base(l, lv)
        {
            PostRepository = postRepository;
        }

        protected bool BeValidUrlFormat(string url)
        {
            var postFid = FacebookHelper.GetGroupPostFid(url);
            return FacebookHelper.IsValidGroupPostUrl(url) && !postFid.IsNullOrWhiteSpace();
        }

        // TODOO Hưng: remove all hardcodes for IG
        /// <summary>
        /// Check valid url format AND existing
        /// </summary>
        /// <param name="urlBatch"></param>
        /// <returns></returns>
        protected async Task<bool> BeValidUrl(string urlBatch)
        {
            if (urlBatch.IsNullOrWhiteSpace()) return false;

            var splitter = "\n";
            var urls = urlBatch.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            if (urls.Count() > MaxUrlReached)
            {
                ismaxUrlReached = true;
                return false;
            }
            
            var countInsUrl = 0;
            for (int i = 0; i < urls.Count; i++)
            {
                if(urls[i].Contains("?")) urls[i] = urls[i].Substring(0, urls[i].IndexOf("?"));
                if (urls[i].Contains("instagram.com")) countInsUrl++;
            }

            if (countInsUrl > 0)
            {
                var invalidInsUrls = urls.Where(_ => !_.Contains("instagram.com")).ToList();
                if (invalidInsUrls.IsNotNullOrEmpty())
                {
                    IsNotSameTypeUrl = invalidInsUrls;
                    return false;
                }
            }
            var invalidUrls = urls.Where(_ => !FacebookHelper.IsValidGroupPostUrl(_)).ToList();
            if (invalidUrls.IsNotNullOrEmpty())
            {
                InvalidUrls = invalidUrls;
                return false;
            }

            return true;
        }
    }

    public class PostCreateDtoValidator : BasePostValidator<PostCreateDto>
    {
        public PostCreateDtoValidator(IPostRepository postRepository, IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv) : base(l, lv, postRepository)
        {
            RuleFor(x => x.Url).MustAsync(async (url, _) => await BeValidUrl(url))
                .WithMessage((dto, url) =>
                {
                    var message = String.Empty;
                    if (ismaxUrlReached)
                    {
                        message += LV[ApiDomainErrorCodes.Posts.MaxUrlReached, MaxUrlReached].Value;
                        return message;
                    }
                    
                    if (IsNotSameTypeUrl.IsNotNullOrEmpty())
                    {
                        message += LV[ApiDomainErrorCodes.Posts.PostSourceTypeMustSame].Value;
                        message += string.Join("\n", IsNotSameTypeUrl);
                    }

                    if (InvalidUrls.IsNotNullOrEmpty())
                    {
                        message = LV[ApiDomainErrorCodes.Posts.InvalidUrl].Value;
                        if (InvalidUrls.IsNotNullOrEmpty())
                        {
                            message += string.Join("\n", InvalidUrls.Select(GetInvalidUrlPath));
                        }
                    }

                    return message;
                });
        }

        private static string GetInvalidUrlPath(string link)
        {
            var url = FacebookHelper.GetCleanUrl(link);
            var path = url.Path.Replace("/groups", "");
            var query = url.Query;
            var value = path + query;

            return value;
        }
    }

    public class PostUpdateDtoValidator : BasePostValidator<PostUpdateDto>
    {
        public PostUpdateDtoValidator(IPostRepository postRepository, IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv) : base(l, lv, postRepository)
        {
            RuleFor(x => x.Url).Must(BeValidUrlFormat)
                .WithMessage((dto, url) =>
                {
                    var message = LV[ApiDomainErrorCodes.Posts.InvalidUrl, url].Value;
                    return message;
                });
        }
    }

}
