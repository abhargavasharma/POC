using System;

namespace TAL.QuoteAndApply.Infrastructure.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundToTwoDecimalPlaces(this decimal val)
        {
            return Math.Round(val, 2, MidpointRounding.AwayFromZero);
        }
    }
}
