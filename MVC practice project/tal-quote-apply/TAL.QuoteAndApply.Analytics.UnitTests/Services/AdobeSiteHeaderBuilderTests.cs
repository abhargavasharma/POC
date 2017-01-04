using System.Reflection;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Analytics.Models;
using TAL.QuoteAndApply.Analytics.Service;
using TAL.QuoteAndApply.Infrastructure.Resource;

namespace TAL.QuoteAndApply.Analytics.UnitTests.Services
{
    public class AdobeSiteHeaderBuilderTests
    {
        private Mock<IResourceFileReader> _mockResourceFileReader;

        [SetUp]
        public void Setup()
        {
            _mockResourceFileReader = new Mock<IResourceFileReader>(MockBehavior.Strict);
        }

        [Test]
        public void GetCode_AdobeHeaderViewModelWithIdentifier_TemplateUpdatedWithModelValue()
        {
            _mockResourceFileReader.Setup(
                call => call.GetContentsOfResource(Assembly.GetAssembly(typeof(Configuration.AnalyticsConfiguration)), "TAL.QuoteAndApply.Analytics.Templates.AdobeSiteHeaderTemplate.cshtml"))
                .Returns(
                    @"@model TAL.QuoteAndApply.Analytics.Models.AdobeHeaderViewModel
<h1>This is a test @Model.UniquePageIdentifier</h1>");

            var service = new AdobeSiteHeaderBuilder(_mockResourceFileReader.Object);

            var content = service.GetCode(new AdobeHeaderViewModel {UniquePageIdentifier = "123456"});

            Assert.That(content, Is.EqualTo(@"<h1>This is a test 123456</h1>"));

        }
    }
}