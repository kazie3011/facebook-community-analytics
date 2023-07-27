using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace FacebookCommunityAnalytics.Api.Utils
{
    public static class ChartJsUtils
    {
        private static readonly Random _rng = new Random();

        public static class ChartColors
        {
            private static readonly Lazy<IReadOnlyList<Color>> _all = new Lazy<IReadOnlyList<Color>>(() => new Color[7] {Red, Orange, Yellow, Green, Blue, Purple, Grey});

            public static IReadOnlyList<Color> All => _all.Value;

            public static readonly Color Red = Color.FromArgb(214, 65, 53);
            public static readonly Color Orange = Color.FromArgb(255, 159, 64);
            public static readonly Color Yellow = Color.FromArgb(247, 200, 63);
            public static readonly Color Green = Color.FromArgb(16, 152, 84);
            public static readonly Color Blue = Color.FromArgb(65, 129, 238);
            public static readonly Color Purple = Color.FromArgb(153, 102, 255);
            public static readonly Color Grey = Color.FromArgb(201, 203, 207);
            public static readonly Color Pink = Color.FromArgb(255, 192, 203);
            public static readonly Color LawnGreen = Color.FromArgb(204, 255, 153);
            
        }

        public static IReadOnlyList<string> Months { get; } = new ReadOnlyCollection<string>
            (new[] {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"});

        private static int RandomScalingFactorThreadUnsafe() => _rng.Next(-100, 100);

        public static int RandomScalingFactor()
        {
            lock (_rng) { return RandomScalingFactorThreadUnsafe(); }
        }

        public static IEnumerable<int> RandomScalingFactor(int count)
        {
            int[] factors = new int[count];
            lock (_rng)
            {
                for (int i = 0; i < count; i++) { factors[i] = RandomScalingFactorThreadUnsafe(); }
            }

            return factors;
        }

        public static IEnumerable<DateTime> GetNextDays(int count)
        {
            DateTime now = DateTime.Now;
            DateTime[] factors = new DateTime[count];
            for (int i = 0; i < factors.Length; i++) { factors[i] = now.AddDays(i); }

            return factors;
        }
    }
}