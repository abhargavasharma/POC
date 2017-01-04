using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class ReferenceApiTests : BaseTestClass<ReferenceClient>
    {
        public ReferenceApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }


        [Test]
        public async Task RetrieveQuote_WhenNoFieldsInRequest_ReturnsErrorModelState_Async()
        {
            //Arrange & Act
            var beneficiaryRelationships = await Client.GetBeneficiaryRelationships(pause: false);

            //Assert
            Assert.That(beneficiaryRelationships, Is.Not.Null);
            Assert.That(beneficiaryRelationships.Count(), Is.EqualTo(9));
        }
    }
}
