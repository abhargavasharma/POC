using System;
using System.Reflection;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Tests.Shared;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Payment.Integration.Tests.Data
{
    [TestFixture, Ignore] //TODO Solve the foreign key constraint
    public class CreditCardPaymentDtoRepositoryTests : DtoRepositoryTests<CreditCardPaymentDto>
    {
        public override BaseRepository<CreditCardPaymentDto> Repo
        {
            get
            {
                var partyConfig = new PaymentConfigurationProvider();
                return new CreditCardPaymentRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            }
        }

        public override CreditCardPaymentDto InsertDto
        {
            get
            {
                return new CreditCardPaymentDto
                {
                    CardNumber = "4111222233334444",
                    CardType = CreditCardType.MasterCard,
                    ExpiryYear = "2018",
                    ExpiryMonth = "05",
                    Token = "++Gibberish++"
                };
            }
        }

        public override Action<CreditCardPaymentDto> UPdateDtoAction
        {
            get { return dto => dto.NameOnCard = "Test"; }
        }

        public override Assembly CallingAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }
    }
}
