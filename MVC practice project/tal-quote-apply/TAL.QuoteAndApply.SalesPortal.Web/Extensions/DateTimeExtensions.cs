using System;
using System.Globalization;

namespace TAL.QuoteAndApply.SalesPortal.Web.Extensions
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

        public static string ToFriendlyDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}