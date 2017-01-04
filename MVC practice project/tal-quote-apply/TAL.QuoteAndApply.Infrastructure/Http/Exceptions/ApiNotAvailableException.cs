using System;

namespace TAL.QuoteAndApply.Infrastructure.Http.Exceptions
{
    public class ApiNotAvailableException : Exception
    {
        public ApiNotAvailableException(string message)
            : base(message)
        {

        }

        public ApiNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}