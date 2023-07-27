using System.Collections.Generic;
using ChartJSCore.Models;

namespace FacebookCommunityAnalytics.Api.Web.Helpers
{
    public static class ChartJsHelper
    {
        public static Dataset GetLineDataset(IList<double?> postsData, int number, string teamName)
        {
            return new LineDataset()
            {
                Data = postsData,
                Label = teamName,
                BackgroundColor = ChartColorHelper.backgroundColors[GetIndexColor(number, ChartColorHelper.backgroundColors.Count)],
                BorderColor = ChartColorHelper.borderColors[GetIndexColor(number, ChartColorHelper.borderColors.Count)],
                Fill = "false",
            };
        }

        private static int GetIndexColor(int index, int colorCount)
        {
            return index + 1 % colorCount;
        }

    }
}