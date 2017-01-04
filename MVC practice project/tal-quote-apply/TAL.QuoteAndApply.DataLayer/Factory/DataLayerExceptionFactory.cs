using System;
using TAL.QuoteAndApply.DataLayer.Exceptions;

namespace TAL.QuoteAndApply.DataLayer.Factory
{
    public interface IDataLayerExceptionFactory
    {
        DataLayerException CreateFrom(Exception exception);
    }

    public class DataLayerExceptionFactory : IDataLayerExceptionFactory
    {
        public DataLayerException CreateFrom(Exception exception)
        {
            if(exception.Message.Contains("Violation of UNIQUE KEY constraint"))
            {
                return new DataLayerUniqueKeyContraintException("Violation of UNIQUE KEY constraint.", exception);
            }

            return new DataLayerException(exception.Message, exception);
        }
    }
}
