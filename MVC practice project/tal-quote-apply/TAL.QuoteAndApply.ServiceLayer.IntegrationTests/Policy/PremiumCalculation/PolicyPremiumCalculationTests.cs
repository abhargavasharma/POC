using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.Tests.Shared.Builders;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.PremiumCalculation
{
    [TestFixture]
    public class PolicyPremiumCalculationTests : BaseServiceLayerTest
    {
        [Test]
        public void CalculateAndSavePolicy_LifeAccident_PremiumsReturnedAtEachLevel_NoDiscounts()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

            var lifePlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "DTH");
            lifePlan.Plan.PremiumType = PremiumType.Stepped;
            lifePlan.Plan.CoverAmount = 100000;

            var lifeAccCover = lifePlan.Covers.First(c => c.Code == "DTHAC");
            lifeAccCover.Selected = true;

            UpdatePolicy(policyWithRisks);
            
            var svc = GetServiceInstance<IPolicyPremiumCalculation>();

            var result = svc.CalculateAndSavePolicy(createQuoteResult.QuoteReference);

            Assert.That(result.PolicyPremium, Is.EqualTo(5.27m));

            var riskSummary = result.RiskPremiums.First();
            Assert.That(riskSummary.RiskPremium, Is.EqualTo(5.27m));
            Assert.That(riskSummary.MultiPlanDiscount, Is.EqualTo(0));

            var lifePlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanSummary.PlanPremium, Is.EqualTo(5.27m));
            Assert.That(lifePlanSummary.PlanIncludingRidersPremium, Is.EqualTo(5.27m));
            Assert.That(lifePlanSummary.MultiCoverDiscount, Is.EqualTo(0));

            var lifeAccSummary = lifePlanSummary.CoverPremiums.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccSummary.CoverPremium, Is.EqualTo(5.27m));
        }

        [Test]
        public void CalculateAndSavePolicy_LifeAccidentAndIllness_PremiumsReturnedAtEachLevel_MultiCoverDiscountReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

            var lifePlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "DTH");
            lifePlan.Plan.PremiumType = PremiumType.Stepped;
            lifePlan.Plan.CoverAmount = 100000;

            var lifeAccCover = lifePlan.Covers.First(c => c.Code == "DTHAC");
            lifeAccCover.Selected = true;

            var lifeIllCover = lifePlan.Covers.First(c => c.Code == "DTHIC");
            lifeIllCover.Selected = true;

            UpdatePolicy(policyWithRisks);

            var svc = GetServiceInstance<IPolicyPremiumCalculation>();

            var result = svc.CalculateAndSavePolicy(createQuoteResult.QuoteReference);

            Assert.That(result.PolicyPremium, Is.EqualTo(13.17m));

            var riskSummary = result.RiskPremiums.First();
            Assert.That(riskSummary.RiskPremium, Is.EqualTo(13.17m));
            Assert.That(riskSummary.MultiPlanDiscount, Is.EqualTo(0));

            var lifePlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanSummary.PlanPremium, Is.EqualTo(13.17m));
            Assert.That(lifePlanSummary.PlanIncludingRidersPremium, Is.EqualTo(13.17m));
            Assert.That(lifePlanSummary.MultiCoverDiscount, Is.EqualTo(2.64m));

            var lifeAccSummary = lifePlanSummary.CoverPremiums.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccSummary.CoverPremium, Is.EqualTo(5.27m));

            var lifeIllSummary = lifePlanSummary.CoverPremiums.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllSummary.CoverPremium, Is.EqualTo(10.54m));
        }

        [Test]
        public void CalculateAndSavePolicy_LifeAccidentAndTpdStandAloneAccident_PremiumsReturnedAtEachLevel_MultiPlanDiscountReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

            var lifePlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "DTH");
            lifePlan.Plan.PremiumType = PremiumType.Stepped;
            lifePlan.Plan.CoverAmount = 100000;
            lifePlan.Plan.Selected = true;

            var lifeAccCover = lifePlan.Covers.First(c => c.Code == "DTHAC");
            lifeAccCover.Selected = true;

            var tpdPlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "TPS");
            tpdPlan.Plan.PremiumType = PremiumType.Stepped;
            tpdPlan.Plan.CoverAmount = 100000;
            tpdPlan.Plan.Selected = true;

             var tpdAccCover = tpdPlan.Covers.First(c => c.Code == "TPSAC");
            tpdAccCover.Selected = true;

            UpdatePolicy(policyWithRisks);

            var svc = GetServiceInstance<IPolicyPremiumCalculation>();

            var result = svc.CalculateAndSavePolicy(createQuoteResult.QuoteReference);

            Assert.That(result.PolicyPremium, Is.EqualTo(8.8m));
            
            var riskSummary = result.RiskPremiums.First();
            Assert.That(riskSummary.RiskPremium, Is.EqualTo(8.8m));
            Assert.That(riskSummary.MultiPlanDiscount, Is.EqualTo(0.46m));
            
            var lifePlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanSummary.PlanPremium, Is.EqualTo(5.01m));
            Assert.That(lifePlanSummary.PlanIncludingRidersPremium, Is.EqualTo(5.01m));
            Assert.That(lifePlanSummary.MultiCoverDiscount, Is.EqualTo(0));

            var lifeAccSummary = lifePlanSummary.CoverPremiums.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccSummary.CoverPremium, Is.EqualTo(5.01m));
            Console.WriteLine(lifeAccSummary.CoverPremium);

            var tpdPlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "TPS");
            Assert.That(tpdPlanSummary.PlanPremium, Is.EqualTo(3.79));
            Assert.That(tpdPlanSummary.PlanIncludingRidersPremium, Is.EqualTo(3.79));
            Assert.That(tpdPlanSummary.MultiCoverDiscount, Is.EqualTo(0));

            var tpdAccSummary = tpdPlanSummary.CoverPremiums.First(c => c.CoverCode == "TPSAC");
            Assert.That(tpdAccSummary.CoverPremium, Is.EqualTo(3.79));
        }

        [Test]
        public void CalculateAndSavePolicy_LifeAccidentAndTpdRiderAccident_PremiumsReturnedAtEachLevelIncludingPlanIncludingRiderPremium_MultiPlanDiscountReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var createQuoteResult = CreateQuoteWithDefaults();
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

            var lifePlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "DTH");
            lifePlan.Plan.PremiumType = PremiumType.Stepped;
            lifePlan.Plan.CoverAmount = 100000;
            lifePlan.Plan.Selected = true;

            var lifeAccCover = lifePlan.Covers.First(c => c.Code == "DTHAC");
            lifeAccCover.Selected = true;

            var tpdPlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == "TPDDTH");
            tpdPlan.Plan.PremiumType = PremiumType.Stepped;
            tpdPlan.Plan.CoverAmount = 100000;
            tpdPlan.Plan.Selected = true;

            var tpdAccCover = tpdPlan.Covers.First(c => c.Code == "TPDDTHAC");
            tpdAccCover.Selected = true;

            UpdatePolicy(policyWithRisks);

            var svc = GetServiceInstance<IPolicyPremiumCalculation>();

            var result = svc.CalculateAndSavePolicy(createQuoteResult.QuoteReference);

            Assert.That(result.PolicyPremium, Is.EqualTo(8.17m));

            var riskSummary = result.RiskPremiums.First();
            Assert.That(riskSummary.RiskPremium, Is.EqualTo(8.17m));
            Assert.That(riskSummary.MultiPlanDiscount, Is.EqualTo(0.43m));
            

            var lifePlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanSummary.PlanPremium, Is.EqualTo(5.01m));
            Assert.That(lifePlanSummary.PlanIncludingRidersPremium, Is.EqualTo(8.17m));
            Assert.That(lifePlanSummary.MultiCoverDiscount, Is.EqualTo(0));

            var lifeAccSummary = lifePlanSummary.CoverPremiums.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccSummary.CoverPremium, Is.EqualTo(5.01m));

            var tpdPlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == "TPDDTH");
            Assert.That(tpdPlanSummary.PlanPremium, Is.EqualTo(3.16m));
            Assert.That(tpdPlanSummary.PlanIncludingRidersPremium, Is.EqualTo(3.16m));
            Assert.That(tpdPlanSummary.MultiCoverDiscount, Is.EqualTo(0));

            var tpdAccSummary = tpdPlanSummary.CoverPremiums.First(c => c.CoverCode == "TPDDTHAC");
            Assert.That(tpdAccSummary.CoverPremium, Is.EqualTo(3.16m));
        }

        [Test, Description("DE6460")]
        public void CalculateAndSavePolicy_LifeAndRidersPlusIncome_PremiumReliefTurnedOnForLife_RidersCalculatedCorrectly()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var createQuoteBuilder = new CreateQuoteBuilder()
                .AsAge(38)
                .AsComputerAnalyst();

            var createQuoteResult = CreateQuote(createQuoteBuilder.Build());
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

            var lifePlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            lifePlan.Plan.PremiumType = PremiumType.Level;
            lifePlan.Plan.CoverAmount = 1500000;
            lifePlan.Plan.Selected = true;

            var premRelief = lifePlan.Options.First(o => o.Code == ProductOptionConstants.LifePremiumRelief);
            premRelief.Selected = true;

            var lifeAccCover = lifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAccidentCover);
            lifeAccCover.Selected = true;

            var lifeIllnessCover = lifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeIllnessCover);
            lifeIllnessCover.Selected = true;

            var lifeSportsCover = lifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAdventureSportsCover);
            lifeSportsCover.Selected = true;

            //tpd rider
            var tpdRider = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == ProductRiderConstants.PermanentDisabilityRiderCode);
            tpdRider.Plan.PremiumType = PremiumType.Level;
            tpdRider.Plan.CoverAmount = 1000000;
            tpdRider.Plan.Selected = true;

            var buyBack = tpdRider.Options.First(o => o.Code == ProductOptionConstants.TpdRiderDeathBuyBack);
            buyBack.Selected = true;

            var tpdRiderAccCover = tpdRider.Covers.First(c => c.Code == ProductRiderCoverConstants.TpdRiderAccidentCover);
            tpdRiderAccCover.Selected = true;

            var tpdRiderIllnessCover = tpdRider.Covers.First(c => c.Code == ProductRiderCoverConstants.TpdRiderIllnessCover);
            tpdRiderIllnessCover.Selected = true;

            var tpdRiderSportsCover = tpdRider.Covers.First(c => c.Code == ProductRiderCoverConstants.TpdRiderAdventureSportsCover);
            tpdRiderSportsCover.Selected = true;

            //ci rider
            var ciRider = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == ProductRiderConstants.CriticalIllnessRiderCode);
            ciRider.Plan.PremiumType = PremiumType.Level;
            ciRider.Plan.CoverAmount = 400000;
            ciRider.Plan.Selected = true;

            var ciBuyBack = ciRider.Options.First(o => o.Code == ProductOptionConstants.CiRiderDeathBuyBack);
            ciBuyBack.Selected = true;

            var ciRiderAccCover = ciRider.Covers.First(c => c.Code == ProductRiderCoverConstants.CiRiderSeriousInjuryCover);
            ciRiderAccCover.Selected = true;

            var ciRiderIllnessCover = ciRider.Covers.First(c => c.Code == ProductRiderCoverConstants.CiRiderSeriousIllnessCover);
            ciRiderIllnessCover.Selected = true;

            var ciRiderCancerCover = ciRider.Covers.First(c => c.Code == ProductRiderCoverConstants.CiRiderCancerCover);
            ciRiderCancerCover.Selected = true;

            //ip
            var ipPlan = policyWithRisks.Risks.First().Plans.First(p => p.Plan.Code == ProductPlanConstants.IncomeProtectionPlanCode);
            ipPlan.Plan.PremiumType = PremiumType.Level;
            ipPlan.Plan.CoverAmount = 8125;
            ipPlan.Plan.Selected = true;
            ipPlan.Plan.WaitingPeriod = 2;
            ipPlan.Plan.BenefitPeriod = 5;

            var ipIcOption = ipPlan.Options.First(o => o.Code == ProductOptionConstants.IpIncreasingClaims);
            ipIcOption.Selected = true;

            var ipDoaOption = ipPlan.Options.First(o => o.Code == ProductOptionConstants.IpDayOneAccident);
            ipDoaOption.Selected = true;

            var ipAccidentCover = ipPlan.Covers.First(c => c.Code == ProductCoverConstants.IpAccidentCover);
            ipAccidentCover.Selected = true;

            var ipIllnesCover = ipPlan.Covers.First(c => c.Code == ProductCoverConstants.IpIllnessCover);
            ipIllnesCover.Selected = true;

            var ipSportsCover = ipPlan.Covers.First(c => c.Code == ProductCoverConstants.IpAdventureSportsCover);
            ipSportsCover.Selected = true;

            UpdatePolicy(policyWithRisks);
            SavePlanOptions(policyWithRisks);

            var svc = GetServiceInstance<IPolicyPremiumCalculation>();

            var result = svc.CalculateAndSavePolicy(createQuoteResult.QuoteReference);

            var riskSummary = result.RiskPremiums.First();

            var lifePlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == ProductPlanConstants.LifePlanCode);
            Assert.That(lifePlanSummary.PlanPremium, Is.EqualTo(202.85));

            var tpdRiderSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == ProductRiderConstants.PermanentDisabilityRiderCode);
            Assert.That(tpdRiderSummary.PlanPremium, Is.EqualTo(155.30));

            var ciRiderSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == ProductRiderConstants.CriticalIllnessRiderCode);
            Assert.That(ciRiderSummary.PlanPremium, Is.EqualTo(244.10));

            var ipPlanSummary = riskSummary.PlanPremiums.First(p => p.PlanCode == ProductPlanConstants.IncomeProtectionPlanCode);
            Assert.That(ipPlanSummary.PlanPremium, Is.EqualTo(388.06));
        }

        private void SavePlanOptions(PolicyWithRisks policyWithRisks)
        {
            var optionService = GetServiceInstance<IOptionService>();

            foreach (var riskWrapper in policyWithRisks.Risks)
            {
                foreach (var planWrapper in riskWrapper.Plans)
                {
                    foreach (var option in planWrapper.Options)
                    {
                        optionService.UpdateOption(option);
                    }
                }
            }
        }
    }
}
