using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy
{
    public class UpdatePlanServiceTests : BaseServiceLayerTest
    {

        [TestCase(PremiumType.Stepped)]
        [TestCase(PremiumType.Level)]
        public void UpdatePremiumTypeOnAllPlans_SetToPremiumType_SetsAllPlansToPremiumType(PremiumType premiumType)
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            //Act
            var svc = GetServiceInstance<IUpdatePlanService>();
            svc.UpdatePremiumTypeOnAllPlans(createQuoteResult.QuoteReference, risk.Risk.Id, premiumType);

            //Assert
            policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            var plansForRisk = policyWithRisks.Risks.Single(r => r.Risk.Id == risk.Risk.Id);

            Assert.That(plansForRisk.Plans.All(p => p.Plan.PremiumType == premiumType));

        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void UpdatePremiumTypeOnAllPlans_SetToUnknownPremiumType_ThrowsException()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            //Act, Assert
            var svc = GetServiceInstance<IUpdatePlanService>();
            svc.UpdatePremiumTypeOnAllPlans(createQuoteResult.QuoteReference, risk.Risk.Id, PremiumType.Unknown);
        }

    }
}
