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
    public class SelfManagedSuperFundDtoRepositoryTests : DtoRepositoryTests<SelfManagedSuperFundPaymentDto>
    {
        
        public override BaseRepository<SelfManagedSuperFundPaymentDto> Repo
        {
            get
            {
                var partyConfig = new PaymentConfigurationProvider();
                return new SelfManagedSuperFundPaymentRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            }
        }

        public override SelfManagedSuperFundPaymentDto InsertDto
        {
            get
            {
                return new SelfManagedSuperFundPaymentDto
                {
                    AccountName = "Some guy",
                    AccountNumber = "1122255544",
                    BSBNumber = "111222"
                };
            }
        }

        public override Action<SelfManagedSuperFundPaymentDto> UPdateDtoAction
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