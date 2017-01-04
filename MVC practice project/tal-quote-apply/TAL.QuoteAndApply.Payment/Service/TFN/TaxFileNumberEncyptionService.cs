using System;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Payment.Service.TFN
{
    public interface ITaxFileNumberEncyptionService
    {
        string Encrypt(string taxFileNumber);
    }

    public class TaxFileNumberEncyptionService : ITaxFileNumberEncyptionService
    {
        private readonly IPasEncryptionHttpService _cryptService;
        private readonly ILoggingService _loggingService;

        public TaxFileNumberEncyptionService(IPasEncryptionHttpService cryptService, ILoggingService loggingService)
        {
            _cryptService = cryptService;
            _loggingService = loggingService;
        }

        public string Encrypt(string taxFileNumber)
        {
            try
            {
                var serviceValue = _cryptService.Encrypt(taxFileNumber);

                return serviceValue;
            }
            catch (Exception ex)
            {
                _loggingService.Error("Unable to encrypt TFN", ex);
                throw ex;
            }
        }
    }
}
