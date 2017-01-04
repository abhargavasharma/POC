using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.UnitTests.Models
{
    [TestFixture]
    public class BrandedProductDefinitionBuilderTests
    {
        [Test]
        public void MidMarketProductDefinitionBuilder_DefaultValuesTest()
        {
            var builder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            var productDefinition = builder.BuildProductDefinition("tal");

            Assert.That(productDefinition.MaximumNumberOfBeneficiaries, Is.EqualTo(5));

            Assert.That(productDefinition.PaymentOptions.Any(po => po.PaymentType == PaymentType.DirectDebit));

            var creditCardOptions = productDefinition.PaymentOptions.SingleOrDefault(po => po.PaymentType == PaymentType.CreditCard) as CreditCardPaymentDefinition;
            Assert.That(creditCardOptions, Is.Not.Null);
            Assert.That(creditCardOptions.AvailableCreditCardTypes.Count, Is.EqualTo(2));
        }

        [Test]
        public void MidMarketProductDefinitionBuilder_AvailablePremiumFrequenciesTest()
        {
            var builder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            var productDefinition = builder.BuildProductDefinition("tal");

            Assert.That(productDefinition.AvailablePremiumFrequencies.Count(), Is.EqualTo(4));
            Assert.That(productDefinition.AvailablePremiumFrequencies.All(p => p != PremiumFrequency.Unknown));
        }
    }
}
