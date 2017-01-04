using System;

namespace TAL.QuoteAndApply.Infrastructure.Http.Exceptions
{
    public class ThirdPartyServiceException : Exception
    {
        public ThirdPartyServiceException(string message)
            : base(message)
        {

        }

        public ThirdPartyServiceException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}