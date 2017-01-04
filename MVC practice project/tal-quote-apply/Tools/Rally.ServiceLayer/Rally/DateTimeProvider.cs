using System;

namespace Rally.ServiceLayer.Rally
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}