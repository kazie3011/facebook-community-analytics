using System.Collections.Generic;
using ChartJSCore.Helpers;

namespace FacebookCommunityAnalytics.Api.Web.Helpers
{
    public static class ChartColorHelper
    {
        private static readonly ChartColor Red = ChartColor.FromRgba(214, 65, 53, 0.5f);
        private static readonly ChartColor Orange = ChartColor.FromRgba(255, 159, 64, 0.5f);
        private static readonly ChartColor Yellow = ChartColor.FromRgba(247, 200, 63, 0.5f);
        private static readonly ChartColor Green = ChartColor.FromRgba(16, 152, 84, 0.5f);
        private static readonly ChartColor Blue = ChartColor.FromRgba(65, 129, 238, 0.5f);
        private static readonly ChartColor Purple = ChartColor.FromRgba(153, 102, 255, 0.5f);
        private static readonly ChartColor Grey = ChartColor.FromRgba(201, 203, 207, 0.5f);
        private static readonly ChartColor GreenLight = ChartColor.FromRgba(116, 156, 247, 0.5f);
        private static readonly ChartColor RedBlood = ChartColor.FromRgba(233, 17, 14, 0.5f);
        private static readonly ChartColor Brown = ChartColor.FromRgba(100, 73, 14, 0.5f);
        private static readonly ChartColor Aqua = ChartColor.FromRgba(0, 255, 255, 0.5f);
        private static readonly ChartColor Fuchsia = ChartColor.FromRgba(255, 0, 255, 0.5f);
        private static readonly ChartColor Darkorchid = ChartColor.FromRgba(153, 50, 204, 0.5f);
        public static readonly string PrimaryColor = "#037404";
        public static List<ChartColor> backgroundColors = new List<ChartColor>()
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Grey,
            GreenLight,
            RedBlood,
            Brown,
            Aqua,
            Fuchsia,
            Darkorchid
        };

        public static List<ChartColor> borderColors = new List<ChartColor>()
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Grey,
            GreenLight,
            RedBlood,
            Brown,
            Aqua,
            Fuchsia,
            Darkorchid
        };

        public static List<ChartColor> avgPostsChartColors = new List<ChartColor>()
        {
            Yellow,
            Blue,
            Green,
            Purple,
            Brown,
            Aqua
        };
    }
}