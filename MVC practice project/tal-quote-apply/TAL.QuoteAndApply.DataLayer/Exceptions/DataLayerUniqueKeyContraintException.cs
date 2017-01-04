using System;

namespace TAL.QuoteAndApply.DataLayer.Exceptions
{
    public class DataLayerUniqueKeyContraintException : DataLayerException
    {
        public DataLayerUniqueKeyContraintException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}