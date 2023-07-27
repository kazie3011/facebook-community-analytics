using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Components;
using Microsoft.Build.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Task = System.Threading.Tasks.Task;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
{
    public partial class TikTokTrending
    {
        private List<TrendingDetailDto> TopTrendingDetails { get; set; }
        private DateTimeOffset?         DateFilter         { get; set; }
        private DateTime?               EndDay             { get; set; }
        private DateTime?               StartDay           { get; set; }

        private int _trendingCount = 50;
        private OrderType Type { get; set; }
        private List<TrendingDetailDto> OriginTrendingDetailDtos { get; set; }
        private TiktokTrendingProperty FieldName { get; set; }
        private IconName? Icon { get; set; }
        private bool VisibleRankProperty { get; set; }
        private bool VisibleViewProperty { get; set; }
        private bool VisibleInCreaseProperty { get; set; }


        public TikTokTrending()
        {
            TopTrendingDetails = new List<TrendingDetailDto>();
            Icon = IconName.AlignLeft;
        }

        protected override async Task OnInitializedAsync()
        {
            await InitTimeForHistoryTrending();
            await DoSearch();
        }

        private async Task DoSearch()
        {
            TopTrendingDetails = await _tiktokStatsAppService.GetTopTrendingContentsByDateRange(StartDay,EndDay,_trendingCount,true);
            InitTable();
        }

        private void InitTable()
        {
            SetInvisibleProperty();
            Type = OrderType.Ascending;
            FieldName = TiktokTrendingProperty.Rank;
            OriginTrendingDetailDtos = TopTrendingDetails.Clone();

        }
        private void TiktokOrderType(TiktokTrendingProperty value)
        {
            if (FieldName != value)
            {
                Type = OrderType.Ascending;
                FieldName = value;
            }
            
            SetInvisibleProperty();

            switch (value)
            {
                case TiktokTrendingProperty.Rank:
                {
                    VisibleRankProperty = true;
                    SortRankField();
                    break;
                }
                
                case TiktokTrendingProperty.Increase:
                {
                    VisibleInCreaseProperty = true;
                    SortInCreaseField();
                    break;
                }
                
                case TiktokTrendingProperty.View:
                {
                    VisibleViewProperty = true;
                    SortViewField();
                    break;
                }
            }
            SetIcon();
            SetOrderType();
        }

        private void SetInvisibleProperty()
        {
            VisibleRankProperty = false;
            VisibleViewProperty = false;
            VisibleInCreaseProperty = false;
        }

        public List<TrendingDetailDto> SortRankField()
        {
            
            switch (Type)
            {
                case OrderType.Ascending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderBy(x=>x.Rank).ToList();
                    break;
                }
                case OrderType.Descending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderByDescending(x=>x.Rank).ToList();
                    break;
                }
                case OrderType.Default:
                {
                    TopTrendingDetails = OriginTrendingDetailDtos.Clone();
                    break;
                }
            }
            return TopTrendingDetails;
        }
        
        public List<TrendingDetailDto> SortViewField()
        {
            switch (Type)
            {
                case OrderType.Ascending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderBy(x=>x.View).ToList();
                    break;
                }
                case OrderType.Descending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderByDescending(x=>x.View).ToList();
                    break;
                }
                case OrderType.Default:
                {
                    TopTrendingDetails = OriginTrendingDetailDtos.Clone();
                    break;
                }
            }
            return TopTrendingDetails;
        }
        
        public List<TrendingDetailDto> SortInCreaseField()
        {
            switch (Type)
            {
                case OrderType.Ascending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderBy(x=>x.Increase).ToList();
                    break;
                }
                case OrderType.Descending:
                {
                    TopTrendingDetails = TopTrendingDetails.OrderByDescending(x=>x.Increase).ToList();
                    break;
                }
                case OrderType.Default:
                {
                    TopTrendingDetails = OriginTrendingDetailDtos.Clone();
                    break;
                }
            }
            return TopTrendingDetails;
        }

        private void SetIcon()
        {
            switch (Type)
            {
                case OrderType.Ascending:
                { 
                    Icon = IconName.ArrowUp;
                    return;
                }
                case OrderType.Descending:
                {
                    Icon = IconName.ArrowDown;
                    return;
                }
                case OrderType.Default:
                {
                    Icon = null;
                    return;
                }
            }
        }

        private void  SetOrderType()
        {
            switch (Type)
            {
                case OrderType.Ascending:
                {
                    Type = OrderType.Descending;
                    return;
                }
                case OrderType.Descending:
                {
                    Type = OrderType.Default;
                    return;
                }
                case OrderType.Default:
                {
                    Type = OrderType.Ascending;
                    return;
                }
            }
        }
        
        
        private async Task InitTimeForHistoryTrending()
        {
            var (from, to)    = await GetDefaultDate();
            DateFilter        = from;
            (StartDay,EndDay) = GetDateTimeForApi(from, to);
        }

        private async Task OnDate_Changed(DateTimeOffset? value)
        {
            if (value != null)
            {
                var (startDate, endDate) = await GetDefaultWeekDate();
                if (value >= startDate && value <= endDate)
                {
                    DateFilter        = value;
                    (StartDay,EndDay) = GetDateTimeForApi(DateFilter.Value,  DateFilter.Value.Add(new TimeSpan(23, 59, 59)));
                    await DoSearch();
                }
                else
                {
                    await MessageService.Info(L["TiktokTrending.SelectRangeInNearestWeek"].ToString());
                }
            }

        }
    }
    
}