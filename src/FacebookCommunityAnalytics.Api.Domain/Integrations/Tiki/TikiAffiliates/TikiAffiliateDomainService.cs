using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.Affiliates;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates
{
    public interface ITikiAffiliateDomainService
    {
        Task<TikiAffGetShortlink.Response> GenerateShortlink(TikiAffGetShortlink.Request apiRequest);
        Task<string> MapAffiliateUrl(string shortlink);

        Task GetTikiOrders();
        Task SyncTikiAffiliate();
    }

    public class TikiAffiliateDomainService : BaseDomainService, ITikiAffiliateDomainService
    {
        private readonly IRepository<TikiAffiliate, Guid> _tikiAffRepo;
        private readonly IRepository<TikiAffiliateStat, Guid> _tikiAffiliateStatRepo;
        private readonly IRepository<UserAffiliate, Guid> _userAffRepo;
        private readonly IRepository<TikiAffiliateConversion, Guid> _tikiAffiliateConversionsRepo;

        private readonly ITikiAffiliateApiConsumer _apiConsumer;

        public TikiAffiliateDomainService(
            ITikiAffiliateApiConsumer apiConsumer,
            IRepository<TikiAffiliate, Guid> tikiAffRepo,
            IRepository<TikiAffiliateStat, Guid> tikiAffiliateStatRepo,
            IRepository<UserAffiliate, Guid> userAffRepo,
            IRepository<TikiAffiliateConversion, Guid> tikiAffiliateConversionsRepo)
        {
            _apiConsumer = apiConsumer;
            _tikiAffRepo = tikiAffRepo;
            _tikiAffiliateStatRepo = tikiAffiliateStatRepo;
            _userAffRepo = userAffRepo;
            _tikiAffiliateConversionsRepo = tikiAffiliateConversionsRepo;
        }

        public async Task<string> GetUniqueShortkey()
        {
            string shortKey = "";
            int forceBreakCount = 10;
            int count = 0;

            do
            {
                var randomString = StringHelper.RandomString(GlobalConfiguration.TikiConfiguration.ShortkeyLength);
                var existing = await _tikiAffRepo.FirstOrDefaultAsync(x => x.Shortlink.Contains(randomString));

                if (existing != null)
                {
                    if (count > forceBreakCount) { throw new ApiException($"Can't generate Tiki shortkey. {forceBreakCount} retries reached"); }

                    count++;
                    continue;
                }

                shortKey = randomString;
                break;
            } while (true);

            return shortKey;
        }

        public async Task<TikiAffGetShortlink.Response> GenerateShortlink(TikiAffGetShortlink.Request apiRequest)
        {
            // Shopiness:   {GlobalConsts.BaseAffiliateDomain}/abcdef
            // Tiki:        {GlobalConsts.BaseAffiliateDomain}/tk/1234567
            // ==> https://ti.ki/xyz/LANDING-PAGE-01-2021?
            // TIKI_URI=https%3A%2F%2Ftiki.vn%2Fkhuyen-mai%2Ftiki-sale-tet-2021&
            // utm_term=TAPM.d5301bba-39ff-4638-ba56-a2addefdd68e_TAPP.1bf2c18d-6595-408a-97c3-900998149eac_TAPT.TI1 
            apiRequest.Shortlink = $"{GlobalConsts.BaseAffiliateDomain}/{await GetUniqueShortkey()}";

            var res = _apiConsumer.GetShortlink(apiRequest);
            res.Shortlink = apiRequest.Shortlink;
            if (res.success)
            {
                var newTikiAffiliate = await _tikiAffRepo.InsertAsync
                (
                    new TikiAffiliate
                    {
                        CommunityFid = apiRequest.CommunityFid,
                        PartnerId = apiRequest.PartnerId,
                        CampaignId = apiRequest.CampaignId,
                        UserCode = apiRequest.UserCode,
                        Shortlink = apiRequest.Shortlink,
                        Link = res.Link,
                        AffiliateUrl = res.AffiliateUrl
                    }
                );
            }

            return res;
        }

        public async Task<string> MapAffiliateUrl(string shortlink)
        {
            var notFoundUrl = GlobalConsts.GDLDomain;
            if (shortlink.IsNullOrWhiteSpace()) { return notFoundUrl; }

            shortlink = shortlink.Trim().Trim('/');
            var existing = await _tikiAffRepo.FirstOrDefaultAsync(x => x.Shortlink == shortlink);

            if (existing != null)
            {
                await RecordClick(existing.Shortlink);
                return existing.AffiliateUrl;
            }
            else { return notFoundUrl; }
        }


        private async Task RecordClick(string shortlink)
        {
            if (shortlink.IsNullOrWhiteSpace()) return;

            var stat = await _tikiAffiliateStatRepo.FindAsync(x => x.Shortlink == shortlink && x.CreatedAt == DateTime.UtcNow.Date);
            if (stat == null)
            {
                stat = new TikiAffiliateStat
                {
                    CreatedAt = DateTime.UtcNow.Date,
                    Shortlink = shortlink,
                    Click = 1
                };
                await _tikiAffiliateStatRepo.InsertAsync(stat);
            }
            else
            {
                stat.Click++;
                await _tikiAffiliateStatRepo.UpdateAsync(stat);
            }

            var userAff = await _userAffRepo.FindAsync(x => x.AffiliateUrl == shortlink);
            if (userAff?.AffConversionModel == null) { userAff.AffConversionModel = new AffConversionModel(); }

            userAff.AffConversionModel.ClickCount++;
            await _userAffRepo.UpdateAsync(userAff);
        }

        public async Task GetTikiOrders()
        {
            var dateRange = DateTime.UtcNow.AddDays(-2).To(DateTime.UtcNow).ToList();
            foreach (var date in dateRange) { await GetTikiOrder(date); }
        }

        public async Task GetTikiOrder(DateTime date)
        {
            var limit = 100;
            var offset = 0;
            Boolean isBreak = true;
            var newTikiAffiliateConversions = new List<TikiAffiliateConversion>();
            do
            {
                var request = new TikiAffGetOrder.Request
                {
                    limit = limit,
                    offset = offset,
                    Date = date,
                };
                var res = await _apiConsumer.GetOrder(request);
                if (res.data == null || (res.pagination != null && res.pagination.total < (limit + offset))) isBreak = false;

                if (res.data != null && res.data.Count > 0) 
                {
                    var TikiAffiliateConversions = MapOrderIntoAffConvertion(res);
                    foreach (var TikiAff in TikiAffiliateConversions)
                    {
                        var aff = await _tikiAffiliateConversionsRepo.FirstOrDefaultAsync(x => x.OrderCode == TikiAff.OrderCode && x.SubOrderCode == TikiAff.SubOrderCode);
                        if (aff == null) newTikiAffiliateConversions.Add(TikiAff);
                    }
                }

                offset += limit;
            } while (isBreak);

            if (newTikiAffiliateConversions.Count > 0) await _tikiAffiliateConversionsRepo.InsertManyAsync(newTikiAffiliateConversions);
        }

        private List<TikiAffiliateConversion> MapOrderIntoAffConvertion(TikiAffGetOrder.Response response)
        {
            var TikiAffiliateConversions = new List<TikiAffiliateConversion>();
            foreach (var order in response.data)
            {
                if (order.status == TikiOrderStatus.Confirmed)
                {
                    TikiAffiliateConversions.Add
                    (
                        new TikiAffiliateConversion
                        {
                            UtmTerm = order.utm_term,
                            OrderCode = order.order_code,
                            SubOrderCode = order.sub_order_code,
                            OfferName = order.offer_name,
                            OfferId = order.offer_id,
                            Status = order.status,
                            EventId = order.event_id,
                            ProductAmount = order.product_amount,
                            ProductName = order.product_name,
                            Discount = order.discount,
                            Quantity = order.quantity,
                            CategoryName = order.category_name,
                            CategoryId = order.category_id,
                            BrandId = order.brand_id,
                            BrandName = order.brand_name,
                            SellerId = order.seller_id,
                            SellerName = order.seller_name,
                            CommissionFee = order.commission_fee,
                            SponsorCommissionFee = order.sponsor_commission_fee,
                            NewCustomer = order.new_customer,
                            DateKey = order.date_key,
                        }
                    );
                }
            }

            return TikiAffiliateConversions;
        }

        public async Task SyncTikiAffiliate()
        {
            var dateRange = DateTime.UtcNow.AddDays(-2).To(DateTime.UtcNow).ToList();
            foreach (var date in dateRange) { await SyncTikiAffiliateByDate(date); }
        }

        public async Task SyncTikiAffiliateByDate(DateTime date)
        {
            var tikiAffiliateConversions = await _tikiAffiliateConversionsRepo.GetListAsync(x => x.DateKey != null && x.DateKey == date.ToString("yyyy-MM-dd"));
            var newUserAffiliate = new List<UserAffiliate>();
            foreach (var tikiAffiliate in tikiAffiliateConversions)
            {
                var tikiUtmData = ExtractShortlinkFromUtmTerm(tikiAffiliate.UtmTerm);
                if (tikiUtmData == null || tikiUtmData.ShortLink == null) continue;

                var userAff = _userAffRepo.FirstOrDefault(x => x.AffiliateUrl == tikiUtmData.ShortLink);
                if (userAff == null)
                {
                    userAff = new UserAffiliate
                    {
                        MarketplaceType = MarketplaceType.Tiki,
                        AffiliateProviderType = AffiliateProviderType.Tiki,
                        AffiliateOwnershipType = AffiliateOwnershipType.GDL,
                        Url =  $"{GlobalConsts.BaseAffiliateDomain}/{tikiUtmData.ShortLink}",
                        AffiliateUrl =  $"{GlobalConsts.BaseAffiliateDomain}/{tikiUtmData.ShortLink}",
                        AffConversionModel = new AffConversionModel
                        {
                            ConversionCount = tikiAffiliate.Quantity,
                            ConversionAmount = tikiAffiliate.Quantity * tikiAffiliate.ProductAmount,
                            CommissionAmount = tikiAffiliate.CommissionFee,
                        }
                    };
                    newUserAffiliate.Add(userAff);
                }
            }

            if (newUserAffiliate.Count > 0) await _userAffRepo.InsertManyAsync(newUserAffiliate);
        }

        public TikiUtmData ExtractShortlinkFromUtmTerm(string utmTerm)
        {
            var TikiUtmData = utmTerm.Split("_");
            var tappString = TikiUtmData.FirstOrDefault(x => x.StartsWith("TAPP"));
            if (tappString == null) return null;
            tappString = tappString.Split("TAPP.")[1];
            var utmData = tappString.Split("-");

            return new TikiUtmData
            {
                ShortLink = utmData.Length > 1 ? utmData[1] : null,
                UserCode = utmData.Length > 2 ? utmData[2] : null,
                CommunityFid = utmData.Length > 3 ? ParseUtmPart(utmData[3]) : Guid.Empty,
                PartnerId = utmData.Length > 4 ? ParseUtmPart(utmData[4]) : Guid.Empty,
                CampaignId = utmData.Length > 5 ? ParseUtmPart(utmData[5]) : Guid.Empty,
            };
        }

        private Guid ParseUtmPart(string part)
        {
            return (part.IsNullOrEmpty() || part == "0") ? Guid.Empty : Guid.Parse(part);
        }

        public class TikiUtmData
        {
            public string ShortLink { get; set; }
            public string UserCode { get; set; }
            public Guid CommunityFid { get; set; }
            public Guid PartnerId { get; set; }
            public Guid CampaignId { get; set; }
        }
    }
}