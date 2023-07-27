namespace FacebookCommunityAnalytics.Api.Core.Extensions
{
    public static class NumericExtensions
    {
        public static string ToCommaStyle(this decimal value, string zeroString = "---")
        {
            if (value == 0)
            {
                return zeroString;
            }

            return $"{value:N0}";
        }

        public static string ToCommaStyle(this object value, string zeroString = "---")
        {
            decimal.TryParse(value.ToString(), out var number);
            return ToCommaStyle(number, zeroString);
        }

        public static decimal ToNonVATAmount(this decimal vatAmount, decimal vatPercent)
        {
            if (vatPercent <= 0)
            {
                return vatAmount;
            }
        
            var nonVATAmount = vatAmount / (1 + ((decimal)vatPercent / 100));
        
            return nonVATAmount;
        }

        // public static decimal ToVATAmount(this decimal value, int vatPercentage)
        // {
        //     if (vatPercentage <= 0)
        //     {
        //         return value;
        //     }
        //
        //     return value + value * (decimal)vatPercentage / 100;
        // }
    }
}