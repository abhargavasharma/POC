using System;

namespace TAL.QuoteAndApply.ServiceLayer.Exceptions
{
    public class ServiceLayerException : ApplicationException
    {
        public ServiceLayerException(string message) : base(message) { }
    }
}
