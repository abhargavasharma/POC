using System;

namespace TAL.QuoteAndApply.DataLayer.Exceptions
{
    public class DataLayerException : Exception
    {
        public DataLayerException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
