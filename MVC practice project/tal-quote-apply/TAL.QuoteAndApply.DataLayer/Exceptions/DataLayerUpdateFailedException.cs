using System;

namespace TAL.QuoteAndApply.DataLayer.Exceptions
{
    public class DataLayerUpdateFailedException : DataLayerException
    {
        public DataLayerUpdateFailedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}