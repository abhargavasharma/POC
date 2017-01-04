using System;
using System.Reflection;
using NUnit.Framework;
using TAL.QuoteAndApply.Analytics.Service;
using TAL.QuoteAndApply.Infrastructure.Resource;

namespace TAL.QuoteAndApply.Analytics.UnitTests.Services
{
    [TestFixture]
    public class ResourceFileReaderTests
    {
        [Test]
        public void GetContentsOfResource_AdobeSiteHeaderTemplateExits_ReturnsNonEmptyString()
        {
            var service = new ResourceFileReader();
            var content = service.GetContentsOfResource(Assembly.GetAssembly(typeof(Configuration.AnalyticsConfiguration)), "TAL.QuoteAndApply.Analytics.Templates.AdobeSiteHeaderTemplate.cshtml");

            Assert.That(content, Is.Not.Empty);

            Console.WriteLine(content);
        }
    }
}
