using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.Users;
using IdentityServer4.Extensions;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public partial class StaffEvaluationDomainService
    {
        #region SALE

        private async Task DoEval_Sale(int year, int month)
        {
            var team = await _organizationDomainService.GetTeam(TeamMemberConsts.Sale);
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var members = await _userDomainService.GetTeamMembers(team.Id);
            var memberIds = members.Select(x => x.Id).ToList();
            var tnxs = await _contractTransactionRepository.GetListAsync
            (
                tnx => tnx.SalePersonId != null
                       && memberIds.Contains(tnx.SalePersonId.Value)
                       && tnx.PaymentDueDate >= fromDateTime
                       && tnx.PaymentDueDate < toDateTime
            );
            var existingEvaluations = await _staffEvaluationRepository.GetListAsync
            (
                x => x.Month == month
                     && x.Year == year
                     && x.TeamId == team.Id
            );

            var evalsToUpdate = new List<StaffEvaluation>();

            foreach (var user in members)
            {
                Debug.WriteLine($"=====================EVALUATING FOR SALE STAFF: {user.UserName} - {fromDateTime} -> {toDateTime}");
                var existing = existingEvaluations.FirstOrDefault(x => x.AppUserId == user.Id);
                if (existing is null) continue;

                var transactions = tnxs.Where(x => x.SalePersonId == user.Id).ToList();
                var userKPI = await GetKPIConfigs(team.Id, user.Id);

                var totalTransactionAmount = existing.SaleKPIAmount > 0 ? existing.SaleKPIAmount : transactions.Sum(x => x.PartialPaymentValue);
                var requiredContractAmount = userKPI.Sale.PaidContractAmountKPI.GetValueOrDefault() * 1000000;

                var quantityPoint = Math.Min(EvaluationConsts.QuantityMaxPoint, Math.Round(totalTransactionAmount / requiredContractAmount * EvaluationConsts.QuantityMaxPoint, 0));
                var quantityKpiDescription = totalTransactionAmount == 0 && requiredContractAmount == 0 && !transactions.Select(x => x.ContractId).Distinct().Any()
                    ? GlobalConsts.NotApplicable :
                    L["StaffEvaluation.Desc.Sale.QuantityKPI",
                    totalTransactionAmount.ToCommaStyle(),
                    requiredContractAmount.ToCommaStyle(),
                    transactions.Select(x => x.ContractId).Distinct().Count()
                ];

                existing.QuantityKPI = quantityPoint;
                existing.TotalPoint = existing.QuantityKPI + existing.QualityKPI + existing.ReviewPoint;
                existing.QuantityKPIDescription = quantityKpiDescription;
                existing.StaffEvaluationStatus = StaffEvaluationStatus.Automated;

                evalsToUpdate.Add(existing);
            }

            if (evalsToUpdate.IsNotNullOrEmpty())
            {
                await _staffEvaluationRepository.UpdateManyAsync(evalsToUpdate);
            }
        }

        #endregion

        #region TIKTOK

        private async Task DoEval_TikTok(int year, int month)
        {
            var team = await _organizationDomainService.GetTeam(TeamMemberConsts.TikTok);
            var members = await _userDomainService.GetTeamMembers(team.Id);
            var existingUserEvaluations = await _staffEvaluationRepository.GetListAsync(x => x.Month == month && x.Year == year && x.TeamId == team.Id && x.CommunityId == null);

            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var tiktokVideos = await _tiktokRepository.GetListAsync(x => x.CreatedDateTime >= fromDateTime && x.CreatedDateTime < toDateTime);
            tiktokVideos = tiktokVideos.GroupBy(x => x.Url).Select(y => y.OrderByDescending(z => z.TotalCount).First()).ToList();
            var tiktokChannels = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
            
            //Get all old communityEvaluation records
            var communityEvaluations = await _staffEvaluationRepository.GetListAsync(x => x.Month == month && x.Year == year && x.TeamId == team.Id && x.CommunityId != null);

            var channelEvaluations = new List<StaffEvaluation>();
            foreach (var channel in tiktokChannels)
            {
                var eval = communityEvaluations.FirstOrDefault(_ => _.CommunityId == channel.Id);
                var channelEvaluation = await DoEval_TikTokChannel(channel, eval, tiktokVideos, year, month);
                if (channelEvaluation is not null)
                {
                    if (channel.ModeratorIds.IsNullOrEmpty())
                    {
                        channelEvaluations.Add(channelEvaluation);
                    }
                    foreach (var moderatorId in channel.ModeratorIds)
                    {
                        var clone = channelEvaluation.Clone();
                        clone.AppUserId = moderatorId;
                        channelEvaluations.Add(clone);
                    }
                }
            }
            
            //Delete for init again
            await _staffEvaluationRepository.DeleteManyAsync(communityEvaluations);

            if (channelEvaluations.IsNotNullOrEmpty()) await _staffEvaluationRepository.InsertManyAsync(channelEvaluations);
            var staffEvalsToUpdate = new List<StaffEvaluation>();
            foreach (var user in members)
            {
                Debug.WriteLine($"=====================EVALUATING FOR TIKTOK STAFF: {user.UserName} - {fromDateTime} -> {toDateTime}");
                var existingEvaluation = existingUserEvaluations.FirstOrDefault(x => x.AppUserId == user.Id);
                if (existingEvaluation == null) continue;

                var userChannelEvals = channelEvaluations.Where(x => x.AppUserId == user.Id).ToList();
                if (userChannelEvals.IsNotNullOrEmpty())
                {
                    existingEvaluation.QuantityKPI = Math.Round(userChannelEvals.Average(_ => _.QuantityKPI), 1, MidpointRounding.AwayFromZero);
                    existingEvaluation.QualityKPI = Math.Round(userChannelEvals.Average(_ => _.QualityKPI), 1, MidpointRounding.AwayFromZero);
                    existingEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
                    existingEvaluation.QuantityKPIDescription = L["StaffEvaluation.Desc.Tiktok.QuantityKPI", userChannelEvals.Count];
                    existingEvaluation.QualityKPIDescription = L["StaffEvaluation.Desc.Tiktok.QualityKPI", userChannelEvals.Count];
                    existingEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;

                    staffEvalsToUpdate.Add(existingEvaluation);
                }
            }

            if (staffEvalsToUpdate.IsNotNullOrEmpty()) await _staffEvaluationRepository.UpdateManyAsync(staffEvalsToUpdate);
        }

        private async Task<StaffEvaluation> DoEval_TikTokChannel(Group channel, StaffEvaluation eval, List<Tiktok> videos, int year, int month)
        {
            var channelVideos = videos.Where(x => x.ChannelId == channel.Name || x.ChannelId == channel.Fid).ToList();

            var requiredVideoQuantity = channel.TikTokTarget?.TargetVideo ?? 0;
            var quantityKPI = requiredVideoQuantity != 0 ? Math.Min(EvaluationConsts.QuantityMaxPoint, Math.Round(((decimal) channelVideos.Count / requiredVideoQuantity) * EvaluationConsts.QuantityMaxPoint, 0)) : 0;

            var qualifiedFollowerCount = channel.TikTokTarget?.TargetFollower ?? 0;
            var qualityKPI = qualifiedFollowerCount != 0 ? Math.Min(EvaluationConsts.QualityMaxPoint, Math.Round(((decimal) channel.Stats.GroupMembers / qualifiedFollowerCount) * EvaluationConsts.QualityMaxPoint, 0)) : 0;

            var channelQuantityKPIDescription = channelVideos.Count == 0 && requiredVideoQuantity == 0
                ? GlobalConsts.NotApplicable
                : L["StaffEvaluation.Desc.Tiktok.Channel.QuantityKPI",
                    channelVideos.Count.ToCommaStyle(),
                    requiredVideoQuantity.ToCommaStyle()
                ];

            var channelQualityKPIDescription = channel.Stats.GroupMembers == 0 && qualifiedFollowerCount == 0
                ? GlobalConsts.NotApplicable
                : L["StaffEvaluation.Desc.Tiktok.Channel.QualityKPI",
                    channel.Stats.GroupMembers.ToCommaStyle(),
                    qualifiedFollowerCount.ToCommaStyle()
                ];

            var reviewPoint = eval?.ReviewPoint ?? 0;

            var team = await _organizationDomainService.GetTeam(TeamMemberConsts.TikTok);
            return new StaffEvaluation(team.Id, null, year, month)
            {
                CommunityId = channel.Id,
                TotalPoint = qualityKPI + quantityKPI + reviewPoint,
                QuantityKPI = quantityKPI,
                QualityKPI = qualityKPI,
                QuantityKPIDescription = channelQuantityKPIDescription,
                QualityKPIDescription = channelQualityKPIDescription,
                ReviewPoint = reviewPoint,
                DirectorReview = eval?.DirectorReview,
                StaffEvaluationStatus = StaffEvaluationStatus.Automated
            };
        }

        #endregion

        #region CONTENT

        private async Task DoEval_Content(int year, int month, List<PostWithNavigationProperties> postNavs)
        {
            var teamNames = GlobalConfiguration.TeamTypeMapping.Content;
            foreach (var teamName in teamNames)
            {
                var team = await _organizationDomainService.GetTeam(teamName);
                await DoEval_Content(year, month, team, postNavs);
            }
        }

        private async Task DoEval_Content(int year, int month, OrganizationUnit team, List<PostWithNavigationProperties> postNavs)
        {
            var members = await _userDomainService.GetTeamMembers(team.Id);
            var existingEvaluations = await _staffEvaluationRepository.GetListAsync(x => x.Month == month && x.Year == year && x.TeamId == team.Id);
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var evalsToUpdate = new List<StaffEvaluation>();
            var posts = postNavs.Where(_ => _.Post.CreatedDateTime != null).Select(_ => _.Post).ToList();

            foreach (var user in members)
            {
                Debug.WriteLine($"=====================EVALUATING FOR CONTENT STAFFS - {team.DisplayName} - {user.UserName} - {fromDateTime} -> {toDateTime}");
                var existingEvaluation = existingEvaluations.FirstOrDefault(x => x.AppUserId == user.Id);
                if (existingEvaluation == null) continue;

                var userPosts = posts.Where(x => x.AppUserId == existingEvaluation.AppUserId).ToList();

                var userKPI = await GetKPIConfigs(team.Id, user.Id);
                if (userKPI.Content is not null)
                {
                    // var currentKPIRate = GlobalConfiguration.KPIConfig.CurrentRate;
                    // var requiredQuantity = Convert.ToInt32(Math.Round((userKPI.Content.ContentPostQuantity.GetValueOrDefault() * (toDateTime - fromDateTime).Days) * currentKPIRate));
                    
                    var requiredQuantity = userKPI.Content.ContentPostQuantity.GetValueOrDefault();
                    var requiredMinimumReactions = userKPI.Content.MinimumPostReactions.GetValueOrDefault();

                    var quantityPoint = Math.Min(EvaluationConsts.QuantityMaxPoint, Math.Round((decimal) userPosts.Count / requiredQuantity * EvaluationConsts.QuantityMaxPoint, 0));
                    var qualityPoint = Math.Min
                        (EvaluationConsts.QualityMaxPoint, Math.Round((decimal) userPosts.Sum(x => x.TotalCount) / requiredMinimumReactions * EvaluationConsts.QualityMaxPoint, 0));

                    var quantityKpiDescription = userPosts.Count == 0 && requiredQuantity == 0
                        ? GlobalConsts.NotApplicable
                        : L["StaffEvaluation.Desc.Content.QuantityKPI",
                            userPosts.Count.ToCommaStyle(),
                            requiredQuantity.ToCommaStyle()
                        ];
                    var qualityKpiDescription = userPosts.Sum(x => x.TotalCount) == 0 && requiredMinimumReactions == 0
                        ? GlobalConsts.NotApplicable
                        : L["StaffEvaluation.Desc.Content.QualityKPI",
                            userPosts.Sum(x => x.TotalCount).ToCommaStyle(),
                            requiredMinimumReactions.ToCommaStyle()
                        ];

                    existingEvaluation.QuantityKPI = quantityPoint;
                    existingEvaluation.QualityKPI = qualityPoint;
                    existingEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
                    existingEvaluation.QuantityKPIDescription = quantityKpiDescription;
                    existingEvaluation.QualityKPIDescription = qualityKpiDescription;
                    existingEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;

                    evalsToUpdate.Add(existingEvaluation);
                }
            }

            if (evalsToUpdate.IsNotNullOrEmpty())
            {
                await _staffEvaluationRepository.UpdateManyAsync(evalsToUpdate);
            }
        }

        #endregion

        private async Task DoEval_Community(int year, int month, List<PostWithNavigationProperties> navPosts)
        {
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);
            var affConversions = await _affiliateConversionRepository.GetListAsync
                (_ => _.ConversionTime >= fromDateTime.ConvertToUnixTimestamp() && _.ConversionTime < toDateTime.ConvertToUnixTimestamp());

            var affiliateTeamNames = GlobalConfiguration.TeamTypeMapping.Affiliate;
            var seedingTeamNames = GlobalConfiguration.TeamTypeMapping.Seeding;
            foreach (var teamName in affiliateTeamNames.Union(seedingTeamNames))
            {
                var team = await _organizationDomainService.GetTeam(teamName);
                await DoEval_Community
                (
                    year,
                    month,
                    team,
                    navPosts,
                    affConversions
                );
            }
        }

        private async Task DoEval_Community(
            int year,
            int month,
            OrganizationUnit team,
            List<PostWithNavigationProperties> navPosts,
            List<AffiliateConversion> affConversions)
        {
            var members = await _userDomainService.GetTeamMembers(team.Id);
            var existingEvaluations = await _staffEvaluationRepository.GetListAsync(x => x.Month == month && x.Year == year && x.TeamId == team.Id);
            var (fromDateTime, toDateTime) = GetPayrollDateTime(year, month);

            var posts = navPosts.Where(_ => _.Post.CreatedDateTime != null).Select(_ => _.Post).ToList();

            var evalsToUpdate = new List<StaffEvaluation>();
            foreach (var user in members)
            {
                Debug.WriteLine($"=====================EVALUATING FOR COMMUNITY STAFFS - {team.DisplayName} - {user.UserName}- {fromDateTime} -> {toDateTime}");
                var existingEvaluation = existingEvaluations.FirstOrDefault(x => x.AppUserId == user.Id);
                if (existingEvaluation == null
                    || await _userDomainService.IsInRole(user.Id, RoleConsts.Supervisor)
                    || await _userDomainService.IsInRole(user.Id, RoleConsts.Manager)
                    || await _userDomainService.IsInRole(user.Id, RoleConsts.Director)
                    || await _userDomainService.IsInRole(user.Id, RoleConsts.Admin)
                   )
                {
                    continue;
                }

                var userKPI = await GetKPIConfigs(team.Id, user.Id);
                // ? filter by date
                // TODOO : improve later, only load 6 months
                var userAffs = await _userAffiliateRepository.GetListAsync(appUserId: existingEvaluation.AppUserId, createdAtMin: DateTime.UtcNow.AddMonths(-6));
                var userPosts = posts.Where(x => x.AppUserId == existingEvaluation.AppUserId).ToList();

                var communityKpiType = GetCommunityKPIType(userKPI);
                switch (communityKpiType)
                {
                    case CommunityKPIType.Unknown:
                        break;

                    case CommunityKPIType.Seeding:
                    {
                        if (userKPI.Seeding is null)
                        {
                            break;
                        }

                        var evaluatePosts = userPosts.Where(_ => _.PostContentType == PostContentType.Seeding).ToList();
                        var totalReactions = userPosts.Sum(x => x.TotalCount);

                        var requiredQuantity = userKPI.Seeding.SeedingPostQuantity.GetValueOrDefault();
                        int requiredMinimumReactions = userKPI.Seeding.MinimumPostReactions.GetValueOrDefault();

                        var quantityPoint = Math.Min(EvaluationConsts.QuantityMaxPoint, Math.Round((decimal) evaluatePosts.Count / requiredQuantity * EvaluationConsts.QuantityMaxPoint, 0));
                        var qualityPoint = Math.Min(EvaluationConsts.QualityMaxPoint, Math.Round((decimal) totalReactions / requiredMinimumReactions * EvaluationConsts.QualityMaxPoint, 0));

                        var quantityKpiDescription = evaluatePosts.Count == 0 && requiredQuantity == 0
                            ? GlobalConsts.NotApplicable
                            : L["StaffEvaluation.Desc.Seeding.QuantityKPI",
                                evaluatePosts.Count.ToCommaStyle(),
                                requiredQuantity.ToCommaStyle()
                            ];
                        var qualityKpiDescription = totalReactions == 0 && requiredMinimumReactions == 0
                            ? GlobalConsts.NotApplicable :
                            L["StaffEvaluation.Desc.Seeding.QualityKPI",
                            totalReactions.ToCommaStyle(),
                            requiredMinimumReactions.ToCommaStyle()
                        ];

                        existingEvaluation.QuantityKPI = quantityPoint;
                        existingEvaluation.QualityKPI = qualityPoint;
                        existingEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
                        existingEvaluation.QuantityKPIDescription = quantityKpiDescription;
                        existingEvaluation.QualityKPIDescription = qualityKpiDescription;
                        existingEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;
                        evalsToUpdate.Add(existingEvaluation);

                        break;
                    }
                    case CommunityKPIType.Affiliate:
                    {
                        if (userKPI.Affiliate is null)
                        {
                            break;
                        }

                        var affPosts = userPosts.Where(_ => _.PostContentType == PostContentType.Affiliate).ToList();
                        var shortKeys = userAffs.Select(_ => UrlHelper.GetShortKey(_.AffiliateUrl)).ToList();
                        var affConversionCount = affConversions.Count(_ => _.ShortKey.IsIn(shortKeys));

                        var requiredQuantity = userKPI.Affiliate.AffiliatePostQuantity.GetValueOrDefault();
                        var minConversionCount = userKPI.Affiliate.MinConversionCount.GetValueOrDefault();

                        if (requiredQuantity == 0) requiredQuantity = userKPI.Affiliate.AffiliatePostQuantity.GetValueOrDefault();
                        if (minConversionCount == 0) minConversionCount = userKPI.Affiliate.MinConversionCount.GetValueOrDefault();

                        var quantityPoint = Math.Min(EvaluationConsts.QuantityMaxPoint, Math.Round(((decimal) userPosts.Count / requiredQuantity) * EvaluationConsts.QuantityMaxPoint, 0));
                        var qualityPoint = Math.Min(EvaluationConsts.QualityMaxPoint, Math.Round(((decimal) affConversionCount / minConversionCount) * EvaluationConsts.QualityMaxPoint, 0));

                        string quantityKPIDescription = L["StaffEvaluation.Desc.Affiliate.QuantityKPI",
                            affPosts.Count.ToCommaStyle(),
                            requiredQuantity.ToCommaStyle()
                        ];
                        string qualityKPIDescription = L["StaffEvaluation.Desc.Affiliate.QualityKPI",
                            affConversionCount.ToCommaStyle(),
                            minConversionCount.ToCommaStyle()
                        ];

                        existingEvaluation.QuantityKPI = quantityPoint;
                        existingEvaluation.QualityKPI = qualityPoint;
                        existingEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
                        existingEvaluation.QuantityKPIDescription = quantityKPIDescription;
                        existingEvaluation.QualityKPIDescription = qualityKPIDescription;
                        existingEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;
                        evalsToUpdate.Add(existingEvaluation);

                        break;
                    }
                    case CommunityKPIType.Community:
                    {
                        var affPosts = userPosts.Count(x => x.PostContentType == PostContentType.Affiliate);
                        var shortKeys = userAffs.Select(_ => UrlHelper.GetShortKey(_.AffiliateUrl)).ToList();
                        var affConversionCount = affConversions.Count(_ => _.ShortKey.IsIn(shortKeys));

                        var seedingPosts = userPosts.Count(x => x.PostContentType == PostContentType.Seeding);
                        var totalSeedingReaction = userPosts.Where(x => x.PostContentType == PostContentType.Seeding).ToList().Sum(x => x.TotalCount);

                        int affiliatePostQuantity = userKPI.Affiliate.AffiliatePostQuantity.GetValueOrDefault();
                        int minConversionCount = userKPI.Affiliate.MinConversionCount.GetValueOrDefault();

                        int seedingPostQuantity = userKPI.Seeding.SeedingPostQuantity.GetValueOrDefault();
                        int minimumPostReactions = userKPI.Seeding.MinimumPostReactions.GetValueOrDefault();

                        var quantityMaxPoint = EvaluationConsts.QuantityMaxPoint / 2;
                        var qualityMaxPoint = EvaluationConsts.QualityMaxPoint / 2;
                        var quantityPoint = Math.Min(quantityMaxPoint, Math.Round(((decimal) affPosts / affiliatePostQuantity) * quantityMaxPoint, 0))
                                            + Math.Min(quantityMaxPoint, Math.Round(((decimal) seedingPosts / seedingPostQuantity) * quantityMaxPoint, 0));
                        var qualityPoint = Math.Min(qualityMaxPoint, Math.Round(((decimal) affConversionCount / minConversionCount) * qualityMaxPoint, 0))
                                           + Math.Min(qualityMaxPoint, Math.Round(((decimal) totalSeedingReaction / minimumPostReactions) * qualityMaxPoint, 0));


                        string quantityKPIDescription = L["StaffEvaluation.Desc.Affiliate.QuantityKPI",
                                                            affPosts.ToCommaStyle(),
                                                            affiliatePostQuantity.ToCommaStyle()
                                                        ]
                                                        + " "
                                                        + L["StaffEvaluation.Desc.Seeding.QuantityKPI",
                                                            seedingPosts.ToCommaStyle(),
                                                            seedingPostQuantity.ToCommaStyle()
                                                        ];

                        string qualityKPIDescription = L["StaffEvaluation.Desc.Affiliate.QualityKPI",
                                                           affConversionCount.ToCommaStyle(),
                                                           minConversionCount.ToCommaStyle()
                                                       ]
                                                       + " "
                                                       + L["StaffEvaluation.Desc.Seeding.QuantityKPI",
                                                           totalSeedingReaction.ToCommaStyle(),
                                                           minimumPostReactions.ToCommaStyle()
                                                       ];

                        existingEvaluation.QuantityKPI = quantityPoint;
                        existingEvaluation.QualityKPI = qualityPoint;
                        existingEvaluation.TotalPoint = existingEvaluation.QuantityKPI + existingEvaluation.QualityKPI + existingEvaluation.ReviewPoint;
                        existingEvaluation.QuantityKPIDescription = quantityKPIDescription;
                        existingEvaluation.QualityKPIDescription = qualityKPIDescription;
                        existingEvaluation.StaffEvaluationStatus = StaffEvaluationStatus.Automated;
                        evalsToUpdate.Add(existingEvaluation);

                        break;
                    }

                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (evalsToUpdate.IsNotNullOrEmpty())
            {
                await _staffEvaluationRepository.UpdateManyAsync(evalsToUpdate);
            }
        }
    }
}