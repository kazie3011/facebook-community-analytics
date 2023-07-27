using Flurl;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using HtmlAgilityPack;

namespace FacebookCommunityAnalytics.Api.Core.Helpers
{
    public static class FacebookHelper
    {
        private const string CaseInsensitive = "(?i)";
        private const string Prefix = @"^" + CaseInsensitive;

        // /groups = '/groups'
        // [a-zA-Z0-9]+ : one or more a-> A->Z and 0->9
        // /*: zero or more '/'
        // https://m.facebook.com/groups/chiemmandep/permalink/459939598606804/
        private const string GroupPostUrlForm1 = Prefix + @"(/groups/[a-zA-Z0-9\.]+/permalink/[0-9]+/*$)";
        private const string GroupPostUrlForm2 = Prefix + @"(/groups/[a-zA-Z0-9\.]+/posts/[0-9]+/*$)";
        private const string PagePostUrlForm1 = Prefix + @"(/[a-zA-Z0-9\.]+/posts/[0-9]+/*$)";
        private const string PagePostUrlForm2 = Prefix + @"(/permalink.php)";
        private const string PagePostUrlForm3 = Prefix + @"(/[a-zA-Z0-9\.]+/permalink/[0-9]+/*$)";
        private const string FbWatchUrlForm = @"(/watch/)";
        private const string FbVideosUrlForm = @"(/videos/)";
        private const string QueryPagePostUrlForm = Prefix + @"(story_fbid=[0-9]+&id=[0-9]+/*$)";
        // /groups = '/groups'
        // [a-zA-Z0-9]+ : one or more a-> A->Z and 0->9
        // [?]: char '?'
        // ([a-zA-Z0-9]+=[a-zA-Z0-9]+[&]?){0,}:
        //   [a-zA-Z0-9]+=[a-zA-Z0-9]+: one or more 'key=value'
        //   [&]: char '?'
        //   ?: zero or one
        //   {0,}: min 0, max infinity
        // https://m.facebook.com/groups/216672169600216?view=permalink&id=470733930860704
        //public const string GroupPostUrlForm2 = Prefix + "(/groups/[a-zA-Z0-9]+/*$)";
        //https://www.facebook.com/permalink.php?story_fbid=190586583011201&id=100823161987544

        public const string GroupUrlForm = Prefix + @"(/groups/[a-zA-Z0-9\.]+)";

        public static bool IsNotValidGroupPostUrl(string url)
        {
            return !IsValidGroupPostUrl(url);
        }

        public static bool IsValidGroupPostUrl(string url)
        {
            try
            {
                if (url.IsNullOrEmpty()) return false;
                if (!url.StartsWith("https")) return false;

                var u = new Url(url);
                var host = u.Host;
                if (host.Contains("facebook.com") && host.Contains("business.facebook.com")) return false;
                var regexForm1 = new Regex(GroupPostUrlForm1);
                var regexForm2 = new Regex(GroupPostUrlForm2);
                var pagePostForm1 = new Regex(PagePostUrlForm1);
                var pagePostForm2 = new Regex(PagePostUrlForm2);

                if (regexForm1.IsMatch(u.Path)) return regexForm1.IsMatch(u.Path);
                if (regexForm2.IsMatch(u.Path)) return regexForm2.IsMatch(u.Path);
                if (pagePostForm1.IsMatch(u.Path)) return pagePostForm1.IsMatch(u.Path);
                if (pagePostForm2.IsMatch(u.Path)) return pagePostForm2.IsMatch(u.Path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static FacebookGroupModel ExtractGroupAndPostFid(string inputUrl)
        {
            var facebookModel = new FacebookGroupModel();

            var url = new Url(inputUrl);
            var regexForm1 = new Regex(GroupPostUrlForm1);
            var regexForm2 = new Regex(GroupPostUrlForm2);
            var pagePostForm1 = new Regex(PagePostUrlForm1);
            var pagePostForm2 = new Regex(PagePostUrlForm2);
            var queryPagePostForm = new Regex(QueryPagePostUrlForm);
            var regexFbWatchForm = new Regex(FbWatchUrlForm);
            var regexFbVideoForm = new Regex(FbVideosUrlForm);
            if (regexFbWatchForm.IsMatch(url))
            {
                var parts = inputUrl.Split(new[] {"?v="}, StringSplitOptions.RemoveEmptyEntries);
                facebookModel.PostFid = parts[1].Trim('/');
                return facebookModel;
            }

            if (regexFbVideoForm.IsMatch(url))
            {
                var parts = inputUrl.Split(new[] {"videos/"}, StringSplitOptions.RemoveEmptyEntries);
                facebookModel.PostFid = parts[1].Trim('/');
                return facebookModel;
            }

            if (regexForm1.IsMatch(url.Path) || regexForm2.IsMatch(url.Path))
            {
                var parts = url.Path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                facebookModel.GroupFid = parts[1].Trim('/');
                facebookModel.PostFid = parts[3].Trim('/');
                facebookModel.GroupSourceType = GroupSourceType.Group;
            }
            else if (pagePostForm1.IsMatch(url.Path))
            {
                var parts = url.Path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                facebookModel.GroupFid = parts[0].Trim('/');
                facebookModel.PostFid = parts[2].Trim('/');
                facebookModel.GroupSourceType = GroupSourceType.Page;
            }
            else if (pagePostForm2.IsMatch(url.Path) && queryPagePostForm.IsMatch(url.Query))
            {
                var parts = url.Query.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
                facebookModel.PostFid = parts[0].Replace("story_fbid=", "");
                facebookModel.GroupFid = parts[1].Replace("id=", "");
                facebookModel.GroupSourceType = GroupSourceType.Page;
            }
            else
            {
                facebookModel.GroupFid = url.Host.Replace("www.", "").Trim(); //groupid = domain
                facebookModel.PostFid = DateTime.UtcNow.Ticks.ToString(); //postId = a random number
                facebookModel.GroupSourceType = GroupSourceType.Website;
            }

            facebookModel.GroupFid = GetFacebookId(facebookModel.GroupFid);

            return facebookModel;
        }

        public static bool IsValidUrl(string url, PostSourceType postSourceType)
        {
            var u = new Url(url);

            var regexGroup1 = new Regex(GroupPostUrlForm1);
            var regexGroup2 = new Regex(GroupPostUrlForm2);
            var regexPage1 = new Regex(PagePostUrlForm1);
            var regexVideo = new Regex(FbVideosUrlForm);
            var regexFbWatch = new Regex(FbWatchUrlForm);

            switch (postSourceType)
            {
                case PostSourceType.Group:
                    return regexGroup1.IsMatch(u.Path) || regexGroup2.IsMatch(u.Path);
                case PostSourceType.Page:
                    return regexPage1.IsMatch(u.Path);
                case PostSourceType.Website:
                    return true;
                case PostSourceType.Instagram:
                    return true;
                case PostSourceType.Video:
                    return regexVideo.IsMatch(u.Path) || regexFbWatch.IsMatch(u.Path);
                default:
                    throw new ArgumentOutOfRangeException(nameof(postSourceType), postSourceType, null);
            }
        }

        public static bool IsValidUrlPostOfPage(string url)
        {
            if (url.Contains("facebook.com/groups") | url.Contains("instagram.com")) return true;
            var u = new Url(url);
            var regexPage = new Regex(PagePostUrlForm3);
            return !regexPage.IsMatch(u.Path);
        }

        public static GroupSourceType GetGroupSourceTypeWithGroupUrl(string url)
        {
            var u = new Url(url);
            var regexForm = new Regex(GroupUrlForm);
            if (regexForm.IsMatch(u.Path)) return GroupSourceType.Group;
            if (u.Host.Contains("facebook.com")) return GroupSourceType.Page;
            else if (u.Host.Contains("instagram.com")) return GroupSourceType.Instagram;
            else if (u.Host.Contains("tiktok")) return GroupSourceType.Tiktok;
            return GroupSourceType.Website;
        }

        public static string GetGroupFid(string inputUrl)
        {
            return ExtractGroupAndPostFid(inputUrl).GroupFid;
        }

        public static string GetGroupPostFid(string inputUrl)
        {
            return ExtractGroupAndPostFid(inputUrl).PostFid;
        }

        public static GroupSourceType GetGroupSourceTypeWithPostUrl(string inputUrl)
        {
            return ExtractGroupAndPostFid(inputUrl).GroupSourceType;
        }

        public static Url GetCleanUrl(string url)
        {
            //link page: https://www.facebook.com/permalink.php?story_fbid=136049168794132&id=100307385701644
            // => https://www.facebook.com/100307385701644/posts/136049168794132
            var uri = new Uri(url);
            var parameters = HttpUtility.ParseQueryString(uri.Query);
            if (parameters.AllKeys.IsNotNullOrEmpty()
                && parameters.AllKeys.Contains(x => x == "story_fbid")
                && parameters.AllKeys.Contains(x => x == "id"))
            {
                //url = $"https://www.facebook.com/{parameters.Get("id")}/posts/{parameters.Get("story_fbid")}";
                url = ExtractLinkGroupOrPage(url);
            }

            //link watch : https://www.facebook.com/watch/?v=316647579942183
            // return link watch
            if (url.Contains("/watch/") || url.Contains("/videos/")) return GetCleanUrlString(url);

            // link page: https://www.facebook.com/100307385701644/photos/a.122159243516458/135593232173059
            // => https://www.facebook.com/100307385701644/posts/122159243516458/
            if (url.Contains("/photos/"))
            {
                var photoUrl = new Url(url);
                var path = photoUrl.Path.Trim('/');
                var items = path.Split('/').ToList();
                path = $"/{items.First()}/posts/{items[items.Count - 2].Split('.')[1]}";
                url = (photoUrl.Root + path).Trim('/');
            }

            // https://facebook.com/lysstudiovn/videos/422844249544896
            // => https://facebook.com/lysstudiovn/posts/422844249544896
            if (url.Contains("/videos/"))
            {
                var photoUrl = new Url(url);
                var path = photoUrl.Path.Trim('/');
                var items = Regex.Split(path, "videos").Select(s => s.Trim('/')).ToList();
                path = $"/{items.First()}/posts/{items.Last()}";
                url = (photoUrl.Root + path).Trim('/');
            }

            var cleanUrl = new Url(url);
            if (cleanUrl.ToString().Contains("/groups/"))
            {
                cleanUrl = url.Replace("/posts/", "/permalink/");
                cleanUrl = (cleanUrl.Root + cleanUrl.Path).Trim('/');
            }

            return GetCleanUrlString(cleanUrl);
        }

        public static bool IsFbVideo(string fbUrl)
        {
            var regexFbWatchForm = new Regex(FbWatchUrlForm);
            var regexFbVideoForm = new Regex(FbVideosUrlForm);

            return regexFbWatchForm.IsMatch(fbUrl) || regexFbVideoForm.IsMatch(fbUrl);
        }

        public static string ExtractLinkGroupOrPage(string sourceUrl)
        {
            sourceUrl = sourceUrl.Replace("www.", "m.");
            if (sourceUrl.StartsWith("https://facebook") || sourceUrl.StartsWith("http://facebook"))
            {
                sourceUrl = sourceUrl.Replace("//facebook", "//m.facebook");
            }

            try
            {
                var htmlWeb = new HtmlWeb()
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.UTF8 //Set UTF8
                };

                //Load data document
                var document = htmlWeb.Load(sourceUrl);

                //Load tag
                var node = document.DocumentNode.SelectSingleNode("//meta[@property='og:url']");

                var link = node.Attributes["content"].Value;

                if (link.IsNotNullOrEmpty())
                {
                    var result = link.Replace("//m.", "//").Replace("www.", string.Empty);
                    return result;
                }

                return sourceUrl;
            }
            catch (Exception)
            {
                return sourceUrl;
            }
        }

        public static string GetCleanUrlString(string url)
        {
            url = url
                .Replace("m.facebook.com", "facebook.com")
                .Replace("https://///", "https://")
                .Replace("https:////", "https://")
                .Replace("https:///", "https://")
                .Replace("www.", "")
                .Replace("/////", "//")
                .Replace("////", "//")
                .Replace("///", "//")
                .Replace("....", ".")
                .Replace("...", ".")
                .Replace("..", ".")
                .Trim()
                .Trim('/')
                .Trim('/');
            if (!url.Contains("instagram") && !url.Contains("/watch/") && !url.Contains("/videos/"))
            {
                if (url.Contains("?")) url = GetPathFromUrl(url);
                url = url.ToLower().Trim('/');
            }

            return url;
        }

        public static string GetPathFromUrl(string url)
        {
            return url.Split('?')[0];
        }

        public static PostSourceType GetPostSourceType(string url)
        {
            var u = new Url(url);
            var groupForm1 = new Regex(GroupPostUrlForm1);
            var groupForm2 = new Regex(GroupPostUrlForm2);
            var pageForm = new Regex(PagePostUrlForm1);

            if (groupForm1.IsMatch(u.Path)) return PostSourceType.Group;
            if (groupForm2.IsMatch(u.Path)) return PostSourceType.Group;
            if (pageForm.IsMatch(u.Path)) return PostSourceType.Page;
            return PostSourceType.Website;
        }

        public static string GetProfileFacebookId(string input)
        {
            if (input.IsNullOrEmpty()) return string.Empty;

            input = input.ToLower().Trim().Trim('/');
            if (input.Contains("facebook.com"))
            {
                var id = string.Empty;
                
                // var numberRegex = new Regex("[0-9]{15,}");
                var fullUrlRegex1 = new Regex("(.*)facebook.com/profile.php[?]id=([a-zA-Z0-9\\.]+)");
                var fullUrlRegex2 = new Regex("(.*)facebook.com/groups/[a-zA-Z0-9\\.]+/user(.*)/[a-zA-Z0-9\\.]+");
                var fullUrlRegex3 = new Regex("(.*)facebook.com/[a-zA-Z0-9\\.]+");

                if (fullUrlRegex1.IsMatch(input))
                {
                    id = new Url(input).QueryParams.FirstOrDefault().Value.ToString();
                }
                else if (fullUrlRegex2.IsMatch(input))
                {
                    id = Regex.Split(input, "/user/").LastOrDefault();
                }
                else if (fullUrlRegex3.IsMatch(input))
                {
                    id = fullUrlRegex3.Match(input).Value.Split('/').LastOrDefault();
                }

                return id;
            }
            else
            {
                return input;
            }
        }

        private static string GetFacebookId(string groupName)
        {
            try
            {
                var htmlWeb = new HtmlWeb
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.UTF8 //Set UTF8
                };

                //Load data document
                var document = htmlWeb.Load($"https://m.facebook.com/groups/{groupName}");

                //Load tag
                var node = document.DocumentNode.SelectSingleNode("//meta[@property='al:android:url']");

                var link = node.Attributes["content"].Value;

                return link.IsNotNullOrEmpty() ? link.Replace("fb://group/", string.Empty) : groupName;
            }
            catch (Exception)
            {
                return groupName;
            }
        }
    }

    public class FacebookGroupModel
    {
        public string PostFid { get; set; }
        public string GroupFid { get; set; }
        public GroupSourceType GroupSourceType { get; set; }
    }
}