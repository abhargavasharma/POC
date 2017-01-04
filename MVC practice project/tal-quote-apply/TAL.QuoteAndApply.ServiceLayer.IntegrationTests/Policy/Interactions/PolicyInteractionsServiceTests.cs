using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.Interactions.Data;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.Interactions
{
    [TestFixture]
    public class PolicyInteractionsServiceTests : BaseServiceLayerTest
    {
        [Test]
        public void PolicyAccessed_WhenPolicyAccessed_InteractionCreated()
        {
            //Arrange
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            //create a policy
            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            var svc = GetServiceInstance<IPolicyInteractionService>();
            //get the interation repo
            var repo = GetServiceInstance<IPolicyInteractionDtoRepository>();

            //Act
            svc.PolicyAccessed(policyWithRisks.Policy.Id);
            //query on policy id
            var result = repo.GetInteractionsByPolicyId(policyWithRisks.Policy.Id);
            
            //Assert
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(result.First().InteractionType, Is.Not.Null);
            Assert.That(result.First().PolicyId, Is.EqualTo(policyWithRisks.Policy.Id));
            Assert.That(result.First().InteractionType, Is.EqualTo(InteractionType.Quote_Accessed));
        }
    }
}
