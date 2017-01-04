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
    public class SuperAnnuationPaymentDtoRepositoryTests : DtoRepositoryTests<SuperAnnuationPaymentDto>
    {
        public override BaseRepository<SuperAnnuationPaymentDto> Repo
        {
            get
            {
                var partyConfig = new PaymentConfigurationProvider();
                return new SuperAnnuationPaymentRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            }
        }

        public override SuperAnnuationPaymentDto InsertDto
        {
            get
            {
                return new SuperAnnuationPaymentDto
                {
                    FundABN = "1234561234",
                    FundName = "Some Fund",
                    FundUSI = "SomeUSI",
                    MembershipNumber = "22445611",
                    TaxFileNumber = "123123123"
                };
            }
        }

        public override Action<SuperAnnuationPaymentDto> UPdateDtoAction
        {
            get { return dto => dto.MembershipNumber = "22445622"; }
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