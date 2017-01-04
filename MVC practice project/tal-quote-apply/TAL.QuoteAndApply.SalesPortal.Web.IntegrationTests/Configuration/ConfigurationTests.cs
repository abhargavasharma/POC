using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void TestBrandTal()
        {
            var service = new BrandSettingsProvider();
            var brandSettings = service.GetForBrand("TAL");

            Assert.That(brandSettings.Enabled, Is.True);
            Assert.That(brandSettings.RoleSettings.AgentGroup, Is.EqualTo(".uTalConsumerAgent_QA"));
            Assert.That(brandSettings.RoleSettings.UnderwritingGroup, Is.EqualTo(".uTalConsumerUnderwriter_QA"));
            Assert.That(brandSettings.RoleSettings.ReadOnlyGroup, Is.EqualTo(".uTalConsumerReadOnly_QA"));

        }

        [Test]
        public void TestBrandQA()
        {
            var service = new BrandSettingsProvider();
            var brandSettings = service.GetForBrand("QA");

            Assert.That(brandSettings.Enabled, Is.False);
            Assert.That(brandSettings.RoleSettings.AgentGroup, Is.EqualTo(".uQantasConsumerAgent_QA"));
            Assert.That(brandSettings.RoleSettings.UnderwritingGroup, Is.EqualTo(".uTalConsumerUnderwriter_QA"));
            Assert.That(brandSettings.RoleSettings.ReadOnlyGroup, Is.EqualTo(".uQantasConsumerReadOnly_QA"));

        }

        [Test]
        public void TestBrandYB()
        {
            var service = new BrandSettingsProvider();
            var brandSettings = service.GetForBrand("YB");

            Assert.That(brandSettings.Enabled, Is.True);
            Assert.That(brandSettings.RoleSettings.AgentGroup, Is.EqualTo(". uYbConsumerAgent _QA"));
            Assert.That(brandSettings.RoleSettings.UnderwritingGroup, Is.EqualTo(".uTalConsumerUnderwriter_QA"));
            Assert.That(brandSettings.RoleSettings.ReadOnlyGroup, Is.EqualTo(".uYbConsumerReadOnly_QA"));

        }
    }
}
