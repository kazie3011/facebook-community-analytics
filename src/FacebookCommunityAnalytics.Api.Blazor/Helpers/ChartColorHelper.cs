using System.Collections.Generic;
using Blazorise.Charts;

namespace FacebookCommunityAnalytics.Api.Blazor.Helpers
{
    public static class ChartColorHelper
    {
        private static readonly string Red = ChartColor.FromRgba(214, 65, 53, 0.5f);
        private static readonly string Orange = ChartColor.FromRgba(255, 159, 64, 0.5f);
        private static readonly string Yellow = ChartColor.FromRgba(247, 200, 63, 0.5f);
        private static readonly string Green = ChartColor.FromRgba(16, 152, 84, 0.5f);
        private static readonly string Blue = ChartColor.FromRgba(65, 129, 238, 0.5f);
        private static readonly string Purple = ChartColor.FromRgba(153, 102, 255, 0.5f);
        private static readonly string Grey = ChartColor.FromRgba(201, 203, 207, 0.5f);
        private static readonly string GreenLight = ChartColor.FromRgba(116, 156, 247, 0.5f);
        private static readonly string RedBlood = ChartColor.FromRgba(233, 17, 14, 0.5f);
        private static readonly string Brown = ChartColor.FromRgba(100, 73, 14, 0.5f);
        private static readonly string Aqua = ChartColor.FromRgba(0, 255, 255, 0.5f);
        private static readonly string Fuchsia = ChartColor.FromRgba(255, 0, 255, 0.5f);
        private static readonly string Darkorchid = ChartColor.FromRgba(153, 50, 204, 0.5f);
        private static readonly string Teal = ChartColor.FromRgba(0, 128, 128, 0.5f);
        private static readonly string Deeppink = ChartColor.FromRgba(255, 20, 147, 0.5f);
        private static readonly string Mediumvioletred = ChartColor.FromRgba(199, 21, 133, 0.5f); 
        private static readonly string Maroon = ChartColor.FromRgba(128, 0, 0, 0.5f); 
        private static readonly string Scarlet = ChartColor.FromRgba(255, 51, 0, 0.5f);
        private static readonly string Ruby = ChartColor.FromRgba(0, 255, 0, 0.5f);
        private static readonly string LightCyan  = ChartColor.FromRgba(204, 255, 255, 0.5f);
        private static readonly string PaleViolet = ChartColor.FromRgba(204, 153, 255, 0.5f);
        private static readonly string LavenderBlue = ChartColor.FromRgba(204, 204, 255, 0.5f);
        private static readonly string Razzmatazz = ChartColor.FromRgba(255, 0, 102, 0.5f);
        private static readonly string DeepCerise = ChartColor.FromRgba(204, 51, 153, 0.5f);
        private static readonly string Hopbush = ChartColor.FromRgba(204, 102, 153, 0.5f);
        private static readonly string LaserLemon = ChartColor.FromRgba(255, 255, 102, 0.5f);
        private static readonly string FreeSpeechRed = ChartColor.FromRgba(204, 0, 0, 0.5f);
        
        public static readonly string PrimaryColor = "#037404";
        private static List<string> backgroundColors = new List<string>()
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
            Darkorchid,
            Teal,
            Deeppink,
            Mediumvioletred,
            Maroon,
            Scarlet,
            Ruby,
            LightCyan,
            PaleViolet,
            LavenderBlue,
            Razzmatazz,
            DeepCerise,
            Hopbush,
            LaserLemon,
            FreeSpeechRed
        };

        private static List<string> borderColors = new List<string>()
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
            Darkorchid,
            Teal,
            Deeppink,
            Mediumvioletred,
            Maroon,
            Scarlet,
            Ruby,
            LightCyan,
            PaleViolet,
            LavenderBlue,
            Razzmatazz,
            DeepCerise,
            Hopbush,
            LaserLemon,
            FreeSpeechRed
        };

        private static List<string> avgPostsChartColors = new List<string>()
        {
            Yellow,
            Blue,
            Green,
            Purple,
            Brown,
            Aqua
        };
        
        private static int GetIndexColor(int index, int colorCount)
        {
            return index % colorCount;
        }

        public static string GetColor(int index)
        {
           return  backgroundColors[GetIndexColor(index, backgroundColors.Count)];
        }
        public static string GetBorderColor(int index)
        {
           return  borderColors[GetIndexColor(index, borderColors.Count)];
        }
    }
}