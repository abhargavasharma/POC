using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Infrastructure.Time
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDate();
        DateTime GetCurrentDateAndTime();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.Today;
        }

        public DateTime GetCurrentDateAndTime()
        {
            return DateTime.Now;
        }
    }
}
