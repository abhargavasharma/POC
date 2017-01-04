using System;
using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Services
{
    [TestFixture]
    public class GetRiskCalculatorInputServiceTests
    {
        [Test]
        public void GetRiskCalculatorInput_AllPlansPlanActive_AllPlanResultsReturned()
        {
            var riskRequest = new RiskCalculationRequest(1, 20, Gender.Female, true, "AAA", 
                new List<PlanCalculationRequest>()
                {
                    new PlanCalculationRequest("ABC", true, true, 1000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                    new PlanCalculationRequest("DEF", true, true, 1000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                });

            var planResults = new List<PlanPremiumCalculationResult>()
            {
                new PlanPremiumCalculationResult("ABC", 100, 10, 0, 0, null),
                new PlanPremiumCalculationResult("DEF", 100, 10, 0, 0, null)
            };
            
            var svc = new GetRiskCalculatorInputService();
            var result = svc.GetRiskCalculatorInput(riskRequest, planResults);

            Assert.That(result.PlanResultsWithDiscounts.Count, Is.EqualTo(2));

        }

        [Test]
        public void GetRiskCalculatorInput_OnePlanActive_ActivePlanResultsReturned()
        {
            var riskRequest = new RiskCalculationRequest(1, 20, Gender.Female, true, "AAA", 
                new List<PlanCalculationRequest>()
                {
                    new PlanCalculationRequest("ABC", true, true, 1000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                    new PlanCalculationRequest("DEF", false, true, 1000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                });

            var planResults = new List<PlanPremiumCalculationResult>()
            {
                new PlanPremiumCalculationResult("ABC", 100, 10, 0, 0, null),
                new PlanPremiumCalculationResult("DEF", 100, 10, 0, 0, null)
            };

            var svc = new GetRiskCalculatorInputService();
            var result = svc.GetRiskCalculatorInput(riskRequest, planResults);

            Assert.That(result.PlanResultsWithDiscounts.Count, Is.EqualTo(1));
        }
    }
}
