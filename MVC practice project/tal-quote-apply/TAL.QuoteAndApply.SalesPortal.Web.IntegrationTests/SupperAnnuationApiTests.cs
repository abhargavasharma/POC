using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class SupperAnnuationApiTests : BaseTestClass<SuperAnnuationClient>
    {
        public SupperAnnuationApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {

        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        [Test]
        public async Task GetFund_404_ReturnsEmptyList()
        {
            var response = await Client.GetSearchSuperannuationByFundName("Definitely not there");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any(), Is.False);
        }

        [Test]
        public async Task GetFund_MLC_ReturnsSomething()
        {
            var response = await Client.GetSearchSuperannuationByFundName("mlc");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any());
        }
    }
}
