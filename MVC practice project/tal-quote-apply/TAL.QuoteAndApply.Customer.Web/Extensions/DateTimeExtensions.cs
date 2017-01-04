using System;

namespace TAL.QuoteAndApply.Customer.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFriendlyString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToFriendlyString();
            }

            return null;
        }

        public static string ToFriendlyString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}