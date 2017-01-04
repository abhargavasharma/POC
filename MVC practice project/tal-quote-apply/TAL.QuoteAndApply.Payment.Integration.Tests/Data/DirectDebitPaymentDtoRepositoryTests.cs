using System;
using System.Reflection;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Tests.Shared;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Payment.Integration.Tests.Data
{
    [TestFixture, Ignore] //TODO Solve the foreign key constraint
    public class DirectDebitPaymentDtoRepositoryTests : DtoRepositoryTests<DirectDebitPaymentDto>
    {
        
        public override BaseRepository<DirectDebitPaymentDto> Repo
        {
            get
            {
                var partyConfig = new PaymentConfigurationProvider();
                return new DirectDebitPaymentRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            }
        }

        public override DirectDebitPaymentDto InsertDto
        {
            get
            {
                return new DirectDebitPaymentDto
                {
                    AccountName = "Some guy",
                    AccountNumber = "1122255544",
                    BSBNumber = "111222"
                };
            }
        }

        public override Action<DirectDebitPaymentDto> UPdateDtoAction
        {
            get { return dto => dto.BSBNumber = "111333"; }
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