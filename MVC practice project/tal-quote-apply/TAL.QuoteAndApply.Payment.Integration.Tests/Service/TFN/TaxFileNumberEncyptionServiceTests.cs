using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Configuration;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Payment.Service.TFN;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Payment.Integration.Tests.Service.TFN
{
    [TestFixture]
    public class TaxFileNumberEncyptionServiceTests
    {
        [Test]
        public void Encrypt_EnsureTFNCEncrypted()
        {
            var tfn = "12345678";

            var loggingSvc = new MockLoggingService();
            var clientSvc = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
                new HttpRequestMessageSerializer());
            var pasEncryptSvc = new PasEncryptionHttpService(new PasEncryptionConfigurationProvider(), clientSvc, new UrlUtilities(), loggingSvc)  ;

            var svc = new TaxFileNumberEncyptionService(pasEncryptSvc, loggingSvc);
            var result = svc.Encrypt(tfn);
            Console.WriteLine(result);
            Assert.That(result, Is.Not.EqualTo(tfn));
        }
    }
}
